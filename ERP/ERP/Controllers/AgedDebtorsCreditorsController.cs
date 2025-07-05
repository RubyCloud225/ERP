using System;
using Microsoft.AspNetCore.Mvc;
using ERP.Service.controller_services.doubleentry;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgedDebtorsCreditorsController : ControllerBase
    {
        private readonly AgedDebtorsCreditorsService _agedService;

        public AgedDebtorsCreditorsController(AgedDebtorsCreditorsService agedService)
        {
            _agedService = agedService;
        }

        [HttpGet("debtors")]
        public IActionResult GetAgedDebtorsReport([FromQuery] DateTime asOfDate)
        {
            if (asOfDate == default)
            {
                return BadRequest("As of date is required.");
            }

            var report = _agedService.GenerateAgedDebtorsReport(asOfDate);
            return Ok(report);
        }

        [HttpGet("creditors")]
        public IActionResult GetAgedCreditorsReport([FromQuery] DateTime asOfDate)
        {
            if (asOfDate == default)
            {
                return BadRequest("As of date is required.");
            }

            var report = _agedService.GenerateAgedCreditorsReport(asOfDate);
            return Ok(report);
        }
    }
}
