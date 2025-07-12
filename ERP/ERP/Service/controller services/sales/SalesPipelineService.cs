using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Service
{
    public class SalesPipelineService : ISalesPipelineService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SalesPipelineService> _logger;

        public SalesPipelineService(ApplicationDbContext dbContext, ILogger<SalesPipelineService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Guid> AddSalesPipeline(ApplicationDbContext.SalesPipeline salesPipeline)
        {
            try
            {
                salesPipeline.Id = Guid.NewGuid();
                await _dbContext.SalesPipelines.AddAsync(salesPipeline);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("SalesPipeline {Name} added successfully with ID {SalesPipelineId}.", salesPipeline.Name, salesPipeline.Id);
                return salesPipeline.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add sales pipeline {Name}.", salesPipeline.Name);
                return Guid.Empty;
            }
        }

        public async Task<bool> UpdateSalesPipeline(Guid salesPipelineId, ApplicationDbContext.SalesPipeline updatedSalesPipeline)
        {
            try
            {
                var salesPipeline = await _dbContext.SalesPipelines.FindAsync(salesPipelineId);
                if (salesPipeline == null)
                {
                    _logger.LogWarning("SalesPipeline with ID {SalesPipelineId} not found.", salesPipelineId);
                    return false;
                }

                salesPipeline.Name = updatedSalesPipeline.Name;
                salesPipeline.Stage = updatedSalesPipeline.Stage;
                salesPipeline.ExpectedCloseDate = updatedSalesPipeline.ExpectedCloseDate;
                salesPipeline.Amount = updatedSalesPipeline.Amount;
                salesPipeline.CustomerId = updatedSalesPipeline.CustomerId;
                salesPipeline.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("SalesPipeline with ID {SalesPipelineId} updated successfully.", salesPipelineId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update sales pipeline with ID {SalesPipelineId}.", salesPipelineId);
                return false;
            }
        }

        public async Task<bool> DeleteSalesPipeline(Guid salesPipelineId)
        {
            try
            {
                var salesPipeline = await _dbContext.SalesPipelines.FindAsync(salesPipelineId);
                if (salesPipeline == null)
                {
                    _logger.LogWarning("SalesPipeline with ID {SalesPipelineId} not found.", salesPipelineId);
                    return false;
                }

                _dbContext.SalesPipelines.Remove(salesPipeline);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("SalesPipeline with ID {SalesPipelineId} deleted successfully.", salesPipelineId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete sales pipeline with ID {SalesPipelineId}.", salesPipelineId);
                return false;
            }
        }

        public async Task<ApplicationDbContext.SalesPipeline?> GetSalesPipelineByIdAsync(Guid salesPipelineId)
        {
            return await _dbContext.SalesPipelines.FindAsync(salesPipelineId);
        }

        public async Task<List<ApplicationDbContext.SalesPipeline>> GetAllSalesPipelinesAsync()
        {
            return await _dbContext.SalesPipelines.ToListAsync();
        }

        public async Task<SalesPipelineClosedDto> GetClosedSalesCountAsync()
        {
            var grouped = await _dbContext.SalesPipelines
                .AsNoTracking()
                .Where(sp => sp.Stage.ToLower() == "closed")
                .GroupBy(sp => new { Year = sp.ExpectedCloseDate.Year, Month = sp.ExpectedCloseDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var labels = grouped.Select(g => $"{g.Year}-{g.Month:D2}").ToArray();
            var data = grouped.Select(g => (decimal)g.Count).ToArray();

            return new SalesPipelineClosedDto
            {
                Labels = labels,
                Data = data
            };
        }
    }

    public interface ISalesPipelineService
    {
        Task<Guid> AddSalesPipeline(ApplicationDbContext.SalesPipeline salesPipeline);
        Task<bool> UpdateSalesPipeline(Guid salesPipelineId, ApplicationDbContext.SalesPipeline updatedSalesPipeline);
        Task<bool> DeleteSalesPipeline(Guid salesPipelineId);
        Task<ApplicationDbContext.SalesPipeline?> GetSalesPipelineByIdAsync(Guid salesPipelineId);
        Task<List<ApplicationDbContext.SalesPipeline>> GetAllSalesPipelinesAsync();
        Task<SalesPipelineClosedDto> GetClosedSalesCountAsync();
    }
}
