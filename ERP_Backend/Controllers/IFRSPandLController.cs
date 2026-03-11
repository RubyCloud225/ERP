using System;
using Microsoft.AspNetCore.Mvc;
using ERP.Service.controller_services.doubleentry;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IFRSPandLController : ControllerBase
    {
        private readonly IFRSPandLService _pAndLService;

        public IFRSPandLController(IFRSPandLService pAndLService)
        {
            _pAndLService = pAndLService;
        }

        [HttpGet]
        public IActionResult GetProfitAndLoss([FromQuery] DateTime financialYearEnd)
        {
            if (financialYearEnd == default)
            {
                return BadRequest("Financial year end date is required.");
            }

            var profitAndLoss = _pAndLService.GenerateProfitAndLoss(financialYearEnd);
            return Ok(profitAndLoss);
        }
    }
}
