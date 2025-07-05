using System;

namespace ERP.Model
{
    public class GeneralBusinessStrategyReport
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CombinedReportContent { get; set; } = string.Empty;
        public string PitchDeckContent { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
