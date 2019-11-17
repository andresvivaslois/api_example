using GNB.Domain.ControllerWorkers.Interfaces;
using GNB.Domain.DTO;
using GNB.Domain.DTO.Mappers;
using GNB.Domain.Helpers.Interfaces;
using GNB.Infrastructure.Factories;
using GNB.Infrastructure.Models;
using GNB.Infrastructure.UnitOfWork;
using GNB.Infrastructure.UnitOfWork.Interfaces;
using GNB.Utilities.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GNB.Domain.ControllerWorkers
{
    public class BankCW : IBankCW
    {
        //Props
        private UnitOfWork<BankContext> Uow = new UnitOfWork<BankContext>(new BankContextFactory());
        private readonly IRepository<Rates> _ratesRepository;
        private readonly IRepository<Transactions> _transactionsRepository;

        IRequestHelper RequestHelper { get; set; }
        IConfiguration Configuration { get; set; }

        //Constructor
        public BankCW(IRequestHelper requestHelper, IConfiguration configuration)
        {
            _ratesRepository = Uow.GetRepository<Rates>();
            _transactionsRepository = Uow.GetRepository<Transactions>();

            this.RequestHelper = requestHelper;
            this.Configuration = configuration;
        }

        //Methods
        public async Task<List<RatesDTO>> GetRates()
        {
            try
            {
                //TODO: Add constants or resources file to avoid hardcoded strings 
                var url = Configuration.GetValue<string>("AppSettings:UrlRates");
                
                var result = await RequestHelper.GetListFromUrl<RatesDTO>(url);

                if (result != null && result.Any())
                {
                    //Save to DB
                    var success = await SaveRatesToDB(result.Select(x=> DTOMapper.RateDTO_To_Rate(x)).ToList());
                    
                    return result;
                }
                else
                {
                    var fromDB = _ratesRepository.FindAll().Select(x => DTOMapper.Rate_To_RateDTO(x)).ToList();
                    if (fromDB != null && fromDB.Any())
                    {
                        return fromDB.ToList();
                    }
                    else
                    {
                        return new List<RatesDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return new List<RatesDTO>();          
            }
        }

        public async Task<List<TransactionsDTO>> GetTransactions()
        {
            try
            {
                //TODO: Add constants or resources file to avoid hardcoded strings 
                var url = Configuration.GetValue<string>("AppSettings:UrlTransactions");

                var result = await RequestHelper.GetListFromUrl<TransactionsDTO>(url);

                if (result != null && result.Any())
                {
                    //Save to DB
                    var success = await SaveTransactionsToDB(result.Select(x => DTOMapper.TransactionDTO_To_Transaction(x)).ToList()); ;

                    return result;
                }
                else
                {
                    var fromDB = _transactionsRepository.FindAll().Select(x=> DTOMapper.Transaction_To_TransactionDTO(x)).ToList();
                    if (fromDB != null && fromDB.Any())
                    {
                        return fromDB.ToList();
                    }
                    else
                    {
                        return new List<TransactionsDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return new List<TransactionsDTO>();
            }
        }

        public async Task<bool> SaveRatesToDB(List<Rates> lRates)
        {
            var validation = await _ratesRepository.DeleteAndInsert(lRates);
            if (validation)
            {
                validation = this.Uow.Save();
            }

            return validation;
        }

        public async Task<bool> SaveTransactionsToDB(List<Transactions> lTransactions ) 
        {
            var validation = await _transactionsRepository.DeleteAndInsert(lTransactions);
            if (validation)
            {
                validation = this.Uow.Save();
            }

            return validation;
        }

        public async Task<List<TransactionsDTO>> GetTransactionsBySKUinEuro(string SKU)
        {
            //I'm understanding that this method will be called after getting rates and transactions.
            //That's the reason why I'm reading first from DB, to avoid make 2 requests to the URL that provides data.
            try
            {
                //Get rates, first from DB, if there is not data, go to URL
                var rates = _ratesRepository.FindAll().Select(x => DTOMapper.Rate_To_RateDTO(x)).ToList();
                if (!rates.Any())
                {
                    rates = await GetRates();
                }

                var euroRates = GetToEuroRates(rates.ToList());

                //Get Transactions filtering by SKU, first from DB, if there is not data, go to URL
                var transactions = _transactionsRepository.FindAll(x => x.SKU == SKU).Select(x => DTOMapper.Transaction_To_TransactionDTO(x)).ToList();
                if (!transactions.Any())
                {
                    transactions = await GetTransactions();
                    transactions = transactions.ToList().Where(x => x.SKU == SKU).ToList() ?? new List<TransactionsDTO>();
                }

                if (!transactions.Any())
                {
                    //TODO: Add constants or resources file to avoid hardcoded strings 
                    LoggerHelper.LogError("There are not transactions for the required SKU");
                }
                else
                {
                    foreach (var trans in transactions.Where(x => x.Currency != "EUR"))
                    {
                        var rate = euroRates.FirstOrDefault(x => x.From == trans.Currency);
                        if (rate != null)
                        {
                            trans.Amount = RounderHelper.RoundToBankersRounding(trans.Amount * rate.Rate);
                            trans.Currency = "EUR";
                        }
                    }
                }

                return transactions.ToList();

            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return new List<TransactionsDTO>();
            }
        }

        private List<RatesDTO> GetToEuroRates(List<RatesDTO> lRates)
        {
            List<RatesDTO> conversedRates = new List<RatesDTO>();

            try
            {
                List<string> currencies = new List<string>();

                //Add to a list the different currencies
                currencies.AddRange(lRates.Select(x => x.From).Distinct());
                currencies.AddRange(lRates.Select(x => x.To).Distinct());
                currencies = currencies.Where(x => x != "EUR").Distinct().ToList();

                //Find direct conversions that we already have
                foreach (var currency in currencies)
                {
                    conversedRates.AddRange(lRates.Where(x => x.From == currency && x.To == "EUR"));
                }

                //Add to a new list the pending currencies which still doesn't have a conversion to EUR
                var pendingCurrencies = currencies.Except(conversedRates.Select(x => x.From)).ToList();

                //Simple (and dirty) control to avoid possible infinite loop if can't find conversions
                var maxRetry = 10;
                var counter = 0;

                while (pendingCurrencies.Any() && counter < maxRetry)
                {
                    counter++;

                    foreach (var currency in pendingCurrencies)
                    {
                        //find a indirect conversion
                        var currencyConversions = lRates.Where(x => x.From == currency).ToList();
                        var indirectConversion = new RatesDTO();
                        RatesDTO usedRate = null;

                        foreach (var item in currencyConversions)
                        {
                            if ((indirectConversion = conversedRates.FirstOrDefault(x => x.From == item.To)) != null)
                            {
                                usedRate = item;
                                break;
                            }
                        }

                        //Use the indirect conversion to calculate the EUR conversion
                        if (usedRate != null)
                        {
                            conversedRates.Add(new RatesDTO
                            {
                                From = currency,
                                To = "EUR",
                                Rate = RounderHelper.RoundToBankersRounding(usedRate.Rate * indirectConversion.Rate)                                
                            });

                            pendingCurrencies = pendingCurrencies.Except(conversedRates.Select(x => x.From)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
            }

            return conversedRates;
 
        }
    }
}
