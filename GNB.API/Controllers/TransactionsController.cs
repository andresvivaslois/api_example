using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GNB.Domain.ControllerWorkers.Interfaces;
using GNB.Domain.DTO;
using GNB.Infrastructure.Models;
using GNB.Utilities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GNB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        IBankCW TransactionsWorker { get; set; }

        public TransactionsController(IBankCW bankCW)
        {
            TransactionsWorker = bankCW;
        }

        // GET api/Transactions
        [HttpGet]
        public async Task<List<TransactionsDTO>> Get()
        {
            var result = await TransactionsWorker.GetTransactions();

            return result;
        }

        // GET api/Transactions/{SKU}
        [HttpGet("{SKU}")]
        public async Task<JsonResult> Get(string SKU)
        {
            var transactionsInEuro = await TransactionsWorker.GetTransactionsBySKUinEuro(SKU);

            List<object> result = new List<object>();

            if (transactionsInEuro.Any())
            {
                result.Add(transactionsInEuro);
                result.Add(new
                {
                    SKU = SKU,
                    Total_Amount = RounderHelper.RoundToBankersRounding(transactionsInEuro.Sum(x => x.Amount))
                });
            }
            else
            {
                result.Add(new { Message = "There are not transactions for the requested SKU!" });
            }
            
            return new JsonResult(result);
        }
    }
}