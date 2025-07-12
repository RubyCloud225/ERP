using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesPipelineController : ControllerBase
    {
        private readonly ISalesPipelineService _salesPipelineService;
        private readonly ILogger<SalesPipelineController> _logger;

        public SalesPipelineController(ISalesPipelineService salesPipelineService, ILogger<SalesPipelineController> logger)
        {
            _salesPipelineService = salesPipelineService;
            _logger = logger;
        }

        [HttpGet("sales-pipeline-closed")]
        public async Task<ActionResult<SalesPipelineClosedDto>> GetClosedSalesCount()
        {
            try
            {
                var result = await _salesPipelineService.GetClosedSalesCountAsync();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching closed sales count");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
