namespace ERP.Model
{
    public class SalesGrowthRateDto
    {
        public required string[] Labels { get; set; }
        public required decimal[] Data { get; set; }
    }
}
