using System;

namespace ERP.Model
{
    public class CapitalAllocationReport
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string ReportContent { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
