using System;
using System.Collections.Generic;

namespace ERP.Model
{
    public class FinancialDataDto
    {
        public Guid UserId { get; set; }
        public required string CompanyName { get; set; }
        public required string Industry { get; set; }
        public required string Geography { get; set; }
        public required string Currency { get; set; }
        public required string Seasonality { get; set; }
        public Dictionary<string, decimal> FinancialMetrics { get; set; } = new Dictionary<string, decimal>();
        // Add other relevant financial data fields as needed
    }
}
