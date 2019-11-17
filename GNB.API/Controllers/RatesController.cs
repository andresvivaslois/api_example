using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GNB.Domain.ControllerWorkers.Interfaces;
using GNB.Domain.DTO;
using GNB.Infrastructure.Models;
using GNB.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GNB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        IBankCW RatesWorker { get; set; }

        public RatesController(IBankCW bankCW)
        {
            RatesWorker = bankCW;
        }
             
        // GET api/Rates
        [HttpGet]
        public async Task<List<RatesDTO>> Get()
        {
            var result = await RatesWorker.GetRates();

            return result;
        }
    }
}
