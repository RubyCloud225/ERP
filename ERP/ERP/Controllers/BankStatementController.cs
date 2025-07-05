using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankStatementController : ControllerBase
    {
        private readonly IBankStatementService _bankStatementService;

        public BankStatementController(IBankStatementService bankStatementService)
        {
            _bankStatementService = bankStatementService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDbContext.BankStatement>> GetBankStatementById(Guid id)
        {
            var bankStatement = await _bankStatementService.GetBankStatementByIdAsync(id);
            if (bankStatement == null)
            {
                return NotFound();
            }
            return Ok(bankStatement);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ApplicationDbContext.BankStatement>>> GetBankStatementsByUser(Guid userId)
        {
            var bankStatements = await _bankStatementService.GetBankStatementsByUserAsync(userId);
            return Ok(bankStatements);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankStatement(Guid id)
        {
            var success = await _bankStatementService.DeleteBankStatementAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationDbContext.BankStatement>> AmendBankStatement(Guid id, [FromBody] ApplicationDbContext.BankStatement amendedBankStatement)
        {
            if (id != amendedBankStatement.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedBankStatement = await _bankStatementService.AmendBankStatementAsync(amendedBankStatement);
                return Ok(updatedBankStatement);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/reconcile")]
        public async Task<ActionResult<bool>> ReconcileBankStatement(Guid id, [FromBody] decimal userInputBalance)
        {
            var result = await _bankStatementService.ReconcileBankStatementAsync(id, userInputBalance);
            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
