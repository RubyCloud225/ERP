using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERP.Model;
using ERP.Service;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialRatioController : ControllerBase
    {
        private readonly IFinancialRatioService _financialRatioService;

        public FinancialRatioController(IFinancialRatioService financialRatioService)
        {
            _financialRatioService = financialRatioService;
        }

        [HttpPost("profitability")]
        public async Task<IActionResult> CalculateProfitabilityRatios([FromBody] FinancialDataDto financialData)
        {
            try
            {
                var result = await _financialRatioService.CalculateProfitabilityRatiosAsync(financialData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("liquidity")]
        public async Task<IActionResult> CalculateLiquidityRatios([FromBody] FinancialDataDto financialData)
        {
            try
            {
                var result = await _financialRatioService.CalculateLiquidityRatiosAsync(financialData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("efficiency")]
        public async Task<IActionResult> CalculateEfficiencyRatios([FromBody] FinancialDataDto financialData)
        {
            try
            {
                var result = await _financialRatioService.CalculateEfficiencyRatiosAsync(financialData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("valuation")]
        public async Task<IActionResult> CalculateValuationRatios([FromBody] FinancialDataDto financialData)
        {
            try
            {
                var result = await _financialRatioService.CalculateValuationRatiosAsync(financialData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("benchmarks")]
        public async Task<IActionResult> GetBenchmarkRatios([FromQuery] string industry, [FromQuery] string geography)
        {
            try
            {
                var result = await _financialRatioService.GetBenchmarkRatiosAsync(industry, geography);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
