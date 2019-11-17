using GNB.Domain.DTO;
using GNB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GNB.Domain.ControllerWorkers.Interfaces
{
    public interface IBankCW
    {
        Task<List<RatesDTO>> GetRates();
        Task<List<TransactionsDTO>> GetTransactions();
        Task<List<TransactionsDTO>> GetTransactionsBySKUinEuro(string SKU);
    }
}
