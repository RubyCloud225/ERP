using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesPipelineController : ControllerBase
    {
        private readonly ISalesPipelineService _salesPipelineService;

        public SalesPipelineController(ISalesPipelineService salesPipelineService)
        {
            _salesPipelineService = salesPipelineService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalesPipeline([FromBody] ApplicationDbContext.SalesPipeline salesPipeline)
        {
            if (salesPipeline == null)
            {
                return BadRequest("Sales pipeline data is required.");
            }

            var id = await _salesPipelineService.AddSalesPipeline(salesPipeline);
            if (id == Guid.Empty)
            {
                return BadRequest("Failed to create sales pipeline.");
            }

            return CreatedAtAction(nameof(GetSalesPipelineById), new { id = id }, id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesPipelineById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Sales pipeline ID is required.");
            }

            var salesPipeline = await _salesPipelineService.GetSalesPipelineByIdAsync(id);
            if (salesPipeline == null)
            {
                return NotFound();
            }

            return Ok(salesPipeline);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesPipelines()
        {
            var salesPipelines = await _salesPipelineService.GetAllSalesPipelinesAsync();
            return Ok(salesPipelines);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesPipeline(Guid id, [FromBody] ApplicationDbContext.SalesPipeline salesPipeline)
        {
            if (id == Guid.Empty || salesPipeline == null)
            {
                return BadRequest("Invalid sales pipeline data.");
            }

            var result = await _salesPipelineService.UpdateSalesPipeline(id, salesPipeline);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesPipeline(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Sales pipeline ID is required.");
            }

            var result = await _salesPipelineService.DeleteSalesPipeline(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
