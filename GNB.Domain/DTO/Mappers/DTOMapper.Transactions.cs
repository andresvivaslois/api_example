using GNB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Domain.DTO.Mappers
{
    public static partial class DTOMapper
    {
        public static TransactionsDTO Transaction_To_TransactionDTO(Transactions entity)
        {
            TransactionsDTO mappedEntity = new TransactionsDTO
            {
                Amount = entity.Amount,
                Currency = entity.Currency,
                SKU = entity.SKU
            }; 

            return mappedEntity;
        }

        public static Transactions TransactionDTO_To_Transaction(TransactionsDTO entity)
        {
            Transactions mappedEntity = new Transactions
            {
                Amount = entity.Amount,
                Currency = entity.Currency,
                SKU = entity.SKU
            };

            return mappedEntity;
        }

    }
}
