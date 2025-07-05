using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VATReturnController : ControllerBase
    {
        private readonly IVATReturnService _vatReturnService;

        public VATReturnController(IVATReturnService vatReturnService)
        {
            _vatReturnService = vatReturnService;
        }

        [HttpGet]
        public async Task<ActionResult<SalesTaxReturnDto>> GetVatReturn([FromQuery] int year, [FromQuery] int quarter)
        {
            if (year <= 0 || quarter <= 0 || quarter > 4)
            {
                return BadRequest("Invalid year or quarter. Quarter must be between 1 and 4.");
            }

            var vatReturn = await _vatReturnService.GetVatReturnAsync(year, quarter);
            return Ok(vatReturn);
        }
    }
}
