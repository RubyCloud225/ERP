using System;
using Microsoft.AspNetCore.Mvc;
using ERP.Service;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IFRSBalanceSheetController : ControllerBase
    {
        private readonly IIFRSBalanceSheetService _balanceSheetService;

        public IFRSBalanceSheetController(IIFRSBalanceSheetService balanceSheetService)
        {
            _balanceSheetService = balanceSheetService;
        }

        [HttpGet]
        public IActionResult GetBalanceSheet([FromQuery] DateTime financialYearEnd)
        {
            if (financialYearEnd == default)
            {
                return BadRequest("Financial year end date is required.");
            }

            var balanceSheet = _balanceSheetService.GenerateBalanceSheet(financialYearEnd);
            return Ok(balanceSheet);
        }
    }
}
