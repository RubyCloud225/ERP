namespace ERP.Model
{
    public class SalesTaxReturnDto
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public decimal TotalSalesTaxCollected { get; set; }
        public decimal TotalPurchaseTaxPaid { get; set; }
        public decimal NetTaxReturn => TotalSalesTaxCollected - TotalPurchaseTaxPaid;
    }
}
