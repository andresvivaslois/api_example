using GNB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Domain.DTO.Mappers
{
    public static partial class DTOMapper
    {
        //Possible upgrade or improvement: Use Automapper or reflection with anonimous types and flags
        public static RatesDTO Rate_To_RateDTO(Rates entity)
        {
            RatesDTO mappedEntity = new RatesDTO
            {
                From = entity.From,
                To = entity.To,
                Rate = entity.Rate
            };

            return mappedEntity;
        }

        public static Rates RateDTO_To_Rate(RatesDTO entity)
        {
            Rates mappedEntity = new Rates
            {
                From = entity.From,
                To = entity.To,
                Rate = entity.Rate
            };

            return mappedEntity;
        }
    }
}
