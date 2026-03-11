using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ERP.Model;
using static ERP.Model.ApplicationDbContext;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Azure.Core;
using Microsoft.AspNetCore.Razor.Language;

namespace ERP.Service
{
    public interface ILlmService
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task<string> GenerateDocumentCatagoryPromptAsync(DocumentRecord documentRecord);
        Task<parsedSalesInvoiceDto> GeneratePromptFromSalesInvoiceAsync(GenerateSalesInvoiceDto request);
        Task<ParsedPurchaseInvoiceDto> GeneratePromptFromPurchaseInvoiceAsync(ParsedPurchaseInvoiceDto request);
        Task<string> GenerateKpiAndTrendAnalysisAsync(FinancialDataDto financialData);
        Task<VisionMissionReport> GenerateVisionMissionAlignmentReportAsync(Guid userId, string userAims);
        Task<VisionMissionReport> GetVisionMissionReportByIdAsync(Guid id);
        Task<VisionMissionReport> UpdateVisionMissionReportAsync(Guid id, string updatedContent);
        Task<StrategicObjectivesReport> GenerateStrategicObjectivesReportAsync(Guid userId, string userObjectives);
        Task<StrategicObjectivesReport> GetStrategicObjectivesReportByIdAsync(Guid id);
        Task<StrategicObjectivesReport> UpdateStrategicObjectivesReportAsync(Guid id, string updatedContent);
        Task<SWOTAnalysisReport> GenerateSWOTAnalysisReportAsync(Guid userId, string userInput);
        Task<SWOTAnalysisReport> GetSWOTAnalysisReportByIdAsync(Guid id);
        Task<SWOTAnalysisReport> UpdateSWOTAnalysisReportAsync(Guid id, string updatedContent);
        Task<PESTELAnalysisReport> GeneratePESTELAnalysisReportAsync(Guid userId, string userInput);
        Task<PESTELAnalysisReport> GetPESTELAnalysisReportByIdAsync(Guid id);
        Task<PESTELAnalysisReport> UpdatePESTELAnalysisReportAsync(Guid id, string updatedContent);
        Task<MarketEntryReport> GenerateMarketEntryReportAsync(Guid userId, string userInput);
        Task<MarketEntryReport> GetMarketEntryReportByIdAsync(Guid id);
        Task<MarketEntryReport> UpdateMarketEntryReportAsync(Guid id, string updatedContent);
        Task<MarketSizingGrowthReport> GenerateMarketSizingGrowthReportAsync(Guid userId, string userInput);
        Task<MarketSizingGrowthReport> GetMarketSizingGrowthReportByIdAsync(Guid id);
        Task<MarketSizingGrowthReport> UpdateMarketSizingGrowthReportAsync(Guid id, string updatedContent);
        Task<MarketSegmentationTargetingReport> GenerateMarketSegmentationTargetingReportAsync(Guid userId, string userInput);
        Task<MarketSegmentationTargetingReport> GetMarketSegmentationTargetingReportByIdAsync(Guid id);
        Task<MarketSegmentationTargetingReport> UpdateMarketSegmentationTargetingReportAsync(Guid id, string updatedContent);
        Task<MarketLandscapeAnalysisReport> GenerateMarketLandscapeAnalysisReportAsync(Guid userId, string userInput);
        Task<MarketLandscapeAnalysisReport> GetMarketLandscapeAnalysisReportByIdAsync(Guid id);
        Task<MarketLandscapeAnalysisReport> UpdateMarketLandscapeAnalysisReportAsync(Guid id, string updatedContent);
        Task<MarketPersonaReport> GenerateMarketPersonaReportAsync(Guid userId, string userInput);
        Task<MarketPersonaReport> GetMarketPersonaReportByIdAsync(Guid id);
        Task<MarketPersonaReport> UpdateMarketPersonaReportAsync(Guid id, string updatedContent);
        Task<PricingStrategyReport> GeneratePricingStrategyReportAsync(Guid userId, string userInput);
        Task<PricingStrategyReport> GetPricingStrategyReportByIdAsync(Guid id);
        Task<PricingStrategyReport> UpdatePricingStrategyReportAsync(Guid id, string updatedContent);
        Task<GoToMarketReport> GenerateGoToMarketReportAsync(Guid userId, string userInput);
        Task<GoToMarketReport> GetGoToMarketReportByIdAsync(Guid id);
        Task<GoToMarketReport> UpdateGoToMarketReportAsync(Guid id, string updatedContent);
        Task<ProductMarketFitReport> GenerateProductMarketFitReportAsync(Guid userId, string userInput);
        Task<ProductMarketFitReport> GetProductMarketFitReportByIdAsync(Guid id);
        Task<ProductMarketFitReport> UpdateProductMarketFitReportAsync(Guid id, string updatedContent);
        Task<CapitalAllocationReport> GenerateCapitalAllocationReportAsync(Guid userId, string userInput);
        Task<CapitalAllocationReport> GetCapitalAllocationReportByIdAsync(Guid id);
        Task<CapitalAllocationReport> UpdateCapitalAllocationReportAsync(Guid id, string updatedContent);
        Task<ROICReport> GenerateROICReportAsync(Guid userId, string userInput);
        Task<ROICReport> GetROICReportByIdAsync(Guid id);
        Task<ROICReport> UpdateROICReportAsync(Guid id, string updatedContent);
        Task<CostOptimizationBenchmarkingReport> GenerateCostOptimizationBenchmarkingReportAsync(Guid userId, string userInput);
        Task<CostOptimizationBenchmarkingReport> GetCostOptimizationBenchmarkingReportByIdAsync(Guid id);
        Task<CostOptimizationBenchmarkingReport> UpdateCostOptimizationBenchmarkingReportAsync(Guid id, string updatedContent);
        Task<RiskAdjustedReport> GenerateRiskAdjustedReportAsync(Guid userId, string userInput);
        Task<RiskAdjustedReport> GetRiskAdjustedReportByIdAsync(Guid id);
        Task<RiskAdjustedReport> UpdateRiskAdjustedReportAsync(Guid id, string updatedContent);
        Task<MandAModelingReport> GenerateMandAModelingReportAsync(Guid userId, string userInput);
        Task<MandAModelingReport> GetMandAModelingReportByIdAsync(Guid id);
        Task<MandAModelingReport> UpdateMandAModelingReportAsync(Guid id, string updatedContent);
        Task<GeneralBusinessStrategyReport> GenerateGeneralBusinessStrategyReportAsync(Guid userId, string userInput);
        Task<GeneralBusinessStrategyReport> GetGeneralBusinessStrategyReportByIdAsync(Guid id);
        Task<GeneralBusinessStrategyReport> UpdateGeneralBusinessStrategyReportAsync(Guid id, string updatedContent);
        Task<string> GeneratePitchDeckAsync(Guid userId, string userInput);
    }
 
    public class LlmService : ILlmService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;

        public LlmService(HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<GeneralBusinessStrategyReport> GenerateGeneralBusinessStrategyReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a comprehensive general business strategy report based on the following input:\n{userInput}\nPlease provide a clear and concise report integrating all relevant aspects.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new GeneralBusinessStrategyReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CombinedReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.GeneralBusinessStrategyReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<GeneralBusinessStrategyReport> GetGeneralBusinessStrategyReportByIdAsync(Guid id)
        {
            var report = await _dbContext.GeneralBusinessStrategyReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"GeneralBusinessStrategyReport with id {id} not found.");
            }
            return report;
        }

        public async Task<GeneralBusinessStrategyReport> UpdateGeneralBusinessStrategyReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.GeneralBusinessStrategyReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"GeneralBusinessStrategyReport with id {id} not found.");
            }
            report.CombinedReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.GeneralBusinessStrategyReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<string> GeneratePitchDeckAsync(Guid userId, string userInput)
        {
            string prompt = $"Generate a pitch deck content based on the following business strategy input:\n{userInput}\nPlease provide a structured and persuasive pitch deck content.";

            string pitchDeckContent = await GenerateResponseAsync(prompt);

            return pitchDeckContent;
        }

        public async Task<CapitalAllocationReport> GenerateCapitalAllocationReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a capital allocation report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new CapitalAllocationReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.CapitalAllocationReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<CapitalAllocationReport> GetCapitalAllocationReportByIdAsync(Guid id)
        {
            var report = await _dbContext.CapitalAllocationReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"CapitalAllocationReport with id {id} not found.");
            }
            return report;
        }

        public async Task<CapitalAllocationReport> UpdateCapitalAllocationReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.CapitalAllocationReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"CapitalAllocationReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.CapitalAllocationReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<ROICReport> GenerateROICReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a ROIC report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new ROICReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ROICReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<ROICReport> GetROICReportByIdAsync(Guid id)
        {
            var report = await _dbContext.ROICReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"ROICReport with id {id} not found.");
            }
            return report;
        }

        public async Task<ROICReport> UpdateROICReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.ROICReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"ROICReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.ROICReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<CostOptimizationBenchmarkingReport> GenerateCostOptimizationBenchmarkingReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a cost optimization benchmarking report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new CostOptimizationBenchmarkingReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.CostOptimizationBenchmarkingReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<CostOptimizationBenchmarkingReport> GetCostOptimizationBenchmarkingReportByIdAsync(Guid id)
        {
            var report = await _dbContext.CostOptimizationBenchmarkingReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"CostOptimizationBenchmarkingReport with id {id} not found.");
            }
            return report;
        }

        public async Task<CostOptimizationBenchmarkingReport> UpdateCostOptimizationBenchmarkingReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.CostOptimizationBenchmarkingReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"CostOptimizationBenchmarkingReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.CostOptimizationBenchmarkingReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<RiskAdjustedReport> GenerateRiskAdjustedReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a risk adjusted report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new RiskAdjustedReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.RiskAdjustedReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<RiskAdjustedReport> GetRiskAdjustedReportByIdAsync(Guid id)
        {
            var report = await _dbContext.RiskAdjustedReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"RiskAdjustedReport with id {id} not found.");
            }
            return report;
        }

        public async Task<RiskAdjustedReport> UpdateRiskAdjustedReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.RiskAdjustedReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"RiskAdjustedReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.RiskAdjustedReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<MandAModelingReport> GenerateMandAModelingReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft an M&A modeling report with DCF model based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new MandAModelingReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MandAModelingReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<MandAModelingReport> GetMandAModelingReportByIdAsync(Guid id)
        {
            var report = await _dbContext.MandAModelingReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MandAModelingReport with id {id} not found.");
            }
            return report;
        }

        public async Task<MandAModelingReport> UpdateMandAModelingReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.MandAModelingReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MandAModelingReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.MandAModelingReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<MarketEntryReport> GenerateMarketEntryReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a market entry report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new MarketEntryReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MarketEntryReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<MarketEntryReport> GetMarketEntryReportByIdAsync(Guid id)
        {
            var report = await _dbContext.MarketEntryReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketEntryReport with id {id} not found.");
            }
            return report;
        }

        public async Task<MarketEntryReport> UpdateMarketEntryReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.MarketEntryReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketEntryReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.MarketEntryReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<MarketSizingGrowthReport> GenerateMarketSizingGrowthReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a market sizing and growth report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new MarketSizingGrowthReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MarketSizingGrowthReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<MarketSizingGrowthReport> GetMarketSizingGrowthReportByIdAsync(Guid id)
        {
            var report = await _dbContext.MarketSizingGrowthReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketSizingGrowthReport with id {id} not found.");
            }
            return report;
        }

        public async Task<MarketSizingGrowthReport> UpdateMarketSizingGrowthReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.MarketSizingGrowthReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketSizingGrowthReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.MarketSizingGrowthReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<MarketSegmentationTargetingReport> GenerateMarketSegmentationTargetingReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a market segmentation and targeting report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new MarketSegmentationTargetingReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MarketSegmentationTargetingReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<MarketSegmentationTargetingReport> GetMarketSegmentationTargetingReportByIdAsync(Guid id)
        {
            var report = await _dbContext.MarketSegmentationTargetingReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketSegmentationTargetingReport with id {id} not found.");
            }
            return report;
        }

        public async Task<MarketSegmentationTargetingReport> UpdateMarketSegmentationTargetingReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.MarketSegmentationTargetingReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketSegmentationTargetingReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.MarketSegmentationTargetingReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<MarketLandscapeAnalysisReport> GenerateMarketLandscapeAnalysisReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a market landscape analysis report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new MarketLandscapeAnalysisReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MarketLandscapeAnalysisReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<MarketLandscapeAnalysisReport> GetMarketLandscapeAnalysisReportByIdAsync(Guid id)
        {
            var report = await _dbContext.MarketLandscapeAnalysisReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketLandscapeAnalysisReport with id {id} not found.");
            }
            return report;
        }

        public async Task<MarketLandscapeAnalysisReport> UpdateMarketLandscapeAnalysisReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.MarketLandscapeAnalysisReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketLandscapeAnalysisReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.MarketLandscapeAnalysisReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<MarketPersonaReport> GenerateMarketPersonaReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a market persona report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new MarketPersonaReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MarketPersonaReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<MarketPersonaReport> GetMarketPersonaReportByIdAsync(Guid id)
        {
            var report = await _dbContext.MarketPersonaReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketPersonaReport with id {id} not found.");
            }
            return report;
        }

        public async Task<MarketPersonaReport> UpdateMarketPersonaReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.MarketPersonaReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"MarketPersonaReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.MarketPersonaReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<PricingStrategyReport> GeneratePricingStrategyReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a pricing strategy report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new PricingStrategyReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.PricingStrategyReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<PricingStrategyReport> GetPricingStrategyReportByIdAsync(Guid id)
        {
            var report = await _dbContext.PricingStrategyReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"PricingStrategyReport with id {id} not found.");
            }
            return report;
        }

        public async Task<PricingStrategyReport> UpdatePricingStrategyReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.PricingStrategyReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"PricingStrategyReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.PricingStrategyReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<GoToMarketReport> GenerateGoToMarketReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a go to market report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new GoToMarketReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.GoToMarketReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<GoToMarketReport> GetGoToMarketReportByIdAsync(Guid id)
        {
            var report = await _dbContext.GoToMarketReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"GoToMarketReport with id {id} not found.");
            }
            return report;
        }

        public async Task<GoToMarketReport> UpdateGoToMarketReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.GoToMarketReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"GoToMarketReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.GoToMarketReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<ProductMarketFitReport> GenerateProductMarketFitReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a product market fit report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new ProductMarketFitReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ProductMarketFitReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<ProductMarketFitReport> GetProductMarketFitReportByIdAsync(Guid id)
        {
            var report = await _dbContext.ProductMarketFitReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"ProductMarketFitReport with id {id} not found.");
            }
            return report;
        }

        public async Task<ProductMarketFitReport> UpdateProductMarketFitReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.ProductMarketFitReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"ProductMarketFitReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.ProductMarketFitReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<SWOTAnalysisReport> GenerateSWOTAnalysisReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a SWOT analysis report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new SWOTAnalysisReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.SWOTAnalysisReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<SWOTAnalysisReport> GetSWOTAnalysisReportByIdAsync(Guid id)
        {
            var report = await _dbContext.SWOTAnalysisReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"SWOTAnalysisReport with id {id} not found.");
            }
            return report;
        }

        public async Task<SWOTAnalysisReport> UpdateSWOTAnalysisReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.SWOTAnalysisReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"SWOTAnalysisReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.SWOTAnalysisReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<PESTELAnalysisReport> GeneratePESTELAnalysisReportAsync(Guid userId, string userInput)
        {
            string prompt = $"Draft a PESTEL analysis report based on the following input:\n{userInput}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new PESTELAnalysisReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.PESTELAnalysisReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<PESTELAnalysisReport> GetPESTELAnalysisReportByIdAsync(Guid id)
        {
            var report = await _dbContext.PESTELAnalysisReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"PESTELAnalysisReport with id {id} not found.");
            }
            return report;
        }

        public async Task<PESTELAnalysisReport> UpdatePESTELAnalysisReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.PESTELAnalysisReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"PESTELAnalysisReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.PESTELAnalysisReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<StrategicObjectivesReport> GenerateStrategicObjectivesReportAsync(Guid userId, string userObjectives)
        {
            string prompt = $"Draft a strategic objectives and key results report based on the following user objectives:\n{userObjectives}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new StrategicObjectivesReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.StrategicObjectivesReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<StrategicObjectivesReport> GetStrategicObjectivesReportByIdAsync(Guid id)
        {
            var report = await _dbContext.StrategicObjectivesReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"StrategicObjectivesReport with id {id} not found.");
            }
            return report;
        }

        public async Task<StrategicObjectivesReport> UpdateStrategicObjectivesReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.StrategicObjectivesReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"StrategicObjectivesReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.StrategicObjectivesReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<VisionMissionReport> GenerateVisionMissionAlignmentReportAsync(Guid userId, string userAims)
        {
            string prompt = $"Draft a vision and mission alignment report based on the following user aims:\n{userAims}\nPlease provide a clear and concise report.";

            string reportContent = await GenerateResponseAsync(prompt);

            var report = new VisionMissionReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportContent = reportContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.VisionMissionReports.Add(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

        public async Task<VisionMissionReport> GetVisionMissionReportByIdAsync(Guid id)
        {
            var report = await _dbContext.VisionMissionReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"VisionMissionReport with id {id} not found.");
            }
            return report;
        }

        public async Task<VisionMissionReport> UpdateVisionMissionReportAsync(Guid id, string updatedContent)
        {
            var report = await _dbContext.VisionMissionReports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"VisionMissionReport with id {id} not found.");
            }
            report.ReportContent = updatedContent;
            report.UpdatedAt = DateTime.UtcNow;
            _dbContext.VisionMissionReports.Update(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var requestBody = new
            {
                prompt = prompt,
                max_tokens = 2048,
            };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
            var response = await _httpClient.PostAsync("https://api.your-llm-service.com/generate", content); // placeholder
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApplicationDbContext.LlmResponse>(responseBody);
                if (result != null && !string.IsNullOrEmpty(result.Response))
                {
                    return result.Response;
                }
                else
                {
                    return "Error generating response";
                }
            }
            else
            {
                throw new HttpRequestException($"Error calling API: {response.StatusCode}");
            }
        }
        public async Task<string> GenerateDocumentCatagoryPromptAsync(DocumentRecord documentRecord)
        {
            string prompt = $"Analyze the following document content and categorize it:\n\n" +
                            $"Document Name: {documentRecord.BlobName}\n\n" +
                            $"{documentRecord.DocumentContent}" +
                            $"Categorize the document into one of the following categories: Sales Invoice, Purchase Invoice, Bank Statement, Email, Letter, or Miscellaneous.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        // prompt for sales invoices
        public async Task<parsedSalesInvoiceDto> GeneratePromptFromSalesInvoiceAsync(GenerateSalesInvoiceDto request)
        {
            await Task.Delay(1000); // Simulate some processing delay
            decimal netAmount = request.LineItems.Sum(line => line.TotalPrice); // Calculate net amount from line items
            decimal taxRate = 0.20M; // Assuming a fixed tax rate of 20% // TODO Change to user entry
            decimal taxAmount = netAmount * taxRate;
            decimal grossAmount = netAmount + taxAmount;
            string invoiceNumber = $"INV-{DateTime.Now.Year}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}"; // Generate a unique invoice number
            string blobName = $"{invoiceNumber}.pdf"; // Placeholder for blob name, should be replaced with actual blob storage logic
                                                      // Generate a prompt for the LLM to recommend nominal and income accounts based on the sales invoice details
            // Generate a prompt for the LLM to recommend nominal and income accounts based on the sales invoice details

            string prompt = $"Recommend the following details for a Sales Invoice:\n" +
                            $"Document Name: {blobName}\n" +
                            $"Invoice Date: {request.InvoiceDate.ToString("yyyy-MM-dd")}\n" +
                            $"Invoice Number: {invoiceNumber}\n" +
                            $"Customer Name: {request.CustomerName}\n" +
                            $"Customer Address: {request.CustomerAddress}\n" +
                            $"Total Amount: {grossAmount:C} (in currency)\n" +
                            $"Sales Tax: {taxAmount:C} (in currency)\n" +
                            $"Net Amount: {netAmount:C} (in currency)\n" +
                            $"Please recommend a sales invoice to the appropriate nominal and income account based on the above details\n" +
                            $"and the following lines:\n" +
                            $"{string.Join("\n", request.LineItems.Select(line =>
                                $"Line Item: {line.Description}, Quantity: {line.Quantity}, Unit Price: {line.UnitPrice:C}, Total Line Amount: {line.TotalPrice:C}"))}" +
                            $"\n\nPlease provide a detailed response with the recommended nominal and income accounts. Consider the following input:\n" +
                            $"Customer: {{request.CustomerName}}, Address: {request.CustomerAddress}, " +
                            $"Invoice Date: {request.InvoiceDate.ToString("yyyy-MM-dd")}, " +
                            $"Due Date: {request.DueDate.ToString("yyyy-MM-dd")}, " +
                            $"Items: {string.Join(", ", request.LineItems.Select(item => $"{item.Description} (Qty: {item.Quantity}, Price: {item.UnitPrice:C})"))}" +
                            $"If an item's description or the overall nature of the sale suggests a revenue category not represented by typical 'Sales Revenue', propose a new, specific nominal account name and its appropriate type ('Income'). Calculate all amounts accurately." +
                            $"If the customer is not a standard customer, please specify the customer's name and address in the response." +
                            $"Ensure the response includes the following fields:\n" +
                            $"Recommended Receivable Nominal: The nominal account to be used for accounts receivable: {request.RecommendedRecieivableNominal}\n" +
                            $"Recommended Income Account: The income account to be used for the sales revenue: {request.RecommendedRevenueNominal}\n" +
                            $"Recommended Tax Nominal: The tax nominal account to be used for sales tax: {request.RecommendedTaxNominal}\n" +
                            $"\n\nEnsure the response is concise and formatted for easy parsing.";
            // Return a parsedSalesInvoiceDto object as the result
            return new parsedSalesInvoiceDto
            {
                InvoiceNumber = invoiceNumber,
                BlobName = blobName,
                TotalAmount = grossAmount,
                SalesTax = taxAmount,
                NetAmount = netAmount,
                CustomerName = request.CustomerName,
                CustomerAddress = request.CustomerAddress,
                RecommendedRecieivableNominal = request.RecommendedRecieivableNominal, // Placeholder for recommended nominal account
                RecommendedRecieivableNominalType = NominalAccountType.Asset, // Placeholder for recommended nominal type
                RecommendedRevenueNominal = request.RecommendedRevenueNominal, // Placeholder for recommended income account
                RecommendedRevenueNominalType = NominalAccountType.Revenue, // Placeholder for recommended income typ
                RecommendedTaxNominal = request.RecommendedTaxNominal, // Placeholder for recommended tax nominal
                RecommendedTaxNominalType = NominalAccountType.Liability, // Placeholder for recommended tax nominal type
            };
        }
        public async Task<ParsedPurchaseInvoiceDto> GeneratePromptFromPurchaseInvoiceAsync(ParsedPurchaseInvoiceDto request)
        {
            string prompt = $"You are an expert accounting AI. I have a purchase invoice with the following details:\n\n" +
                            $"Document Name: {request.BlobName}\n" +
                            $"Invoice Name: {request.InvoiceNumber}\n" +
                            $"Invoice Date: {request.InvoiceDate.ToString("yyyy-MM-dd")}\n" +
                            $"SupplierName: {request.SupplierName}\n" +
                            $"Supplier Address: {request.Address}\n" +
                            $"Total Amount: {request.TotalAmount:C} (in currency)\n" +
                            $"Purchase Tax: {request.TaxAmount:C} (in currency)\n" +
                            $"Net Amount: {request.NetAmount:C} (in currency)\n" +
                            $"Due Date: {request.DueDate:C}\n" +
                            $"Line Items: {string.Join("\n", request.LineItems.Select(line =>
                                $"Line Item: {line.Description}, Quantity: {line.Quantity}, Unit Price: {line.UnitPrice:C}, Total Line Amount: {line.TotalAmount:C}"))}\n\n" +
                            $"Based on these details, please recommend the appropriate nominal accounts for double-entry accounting record. \n" +
                            $"Focus on the primary nominal accounts for the purchase invoice, including:\n" +
                            $"RecommendedExpenseNominal: {request.RecommendedExpenseNominal}\n" +
                            $"RecommendedPayableNominal: {request.RecommendedPayableNominal}\n" +
                            $"RecommendedTaxNominal: {request.RecommendedTaxNominal}\n" +
                            $"Please recommend a purchase invoice to the appropriate nominal and expense account based on the above details";
            await Task.Delay(1000); // Simulate some processing delay
            return new ParsedPurchaseInvoiceDto
            {
                BlobName = request.BlobName,
                InvoiceNumber = request.InvoiceNumber,
                InvoiceDate = request.InvoiceDate,
                SupplierName = request.SupplierName,
                Address = request.Address,
                TotalAmount = request.TotalAmount,
                TaxAmount = request.TaxAmount,
                NetAmount = request.NetAmount,
                DueDate = request.DueDate,
                LineItems = request.LineItems,
                RecommendedExpenseNominal = request.RecommendedExpenseNominal,
                RecommendedPayableNominal = request.RecommendedPayableNominal,
                RecommendedTaxNominal = request.RecommendedTaxNominal
            };
        }

        public async Task<string> GenerateKpiAndTrendAnalysisAsync(FinancialDataDto financialData)
        {
            string prompt = $"You are a market analyst AI. Based on the following financial data, assess the industry of the company and provide KPIs and trend analysis of its competitors in the market.\n\n" +
                            $"Company Name: {financialData.CompanyName}\n" +
                            $"Industry: {financialData.Industry}\n" +
                            $"Geography: {financialData.Geography}\n" +
                            $"Currency: {financialData.Currency}\n" +
                            $"Seasonality: {financialData.Seasonality}\n" +
                            $"Financial Metrics:\n";

            foreach (var metric in financialData.FinancialMetrics)
            {
                prompt += $"- {metric.Key}: {metric.Value}\n";
            }

            prompt += "\nPlease provide a detailed market analysis including key performance indicators, competitor trends, and relevant industry insights.";

            string response = await GenerateResponseAsync(prompt);
            return response;
        }

        
        
    }
}
