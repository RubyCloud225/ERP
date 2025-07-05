using System;

namespace ERP.Model
{
    public class MandAModelingReport
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ReportContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
