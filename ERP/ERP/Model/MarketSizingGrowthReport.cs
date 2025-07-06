using System;

namespace ERP.Model
{
    public class MarketSizingGrowthReport
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string ReportContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
