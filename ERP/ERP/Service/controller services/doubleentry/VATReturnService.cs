using System.Threading.Tasks;
using ERP.Model;

namespace ERP.Service
{
    public interface IVATReturnService
    {
        Task<SalesTaxReturnDto> GetVatReturnAsync(int year, int quarter);
    }

    public class VATReturnService : IVATReturnService
    {
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        public VATReturnService(ISalesInvoiceService salesInvoiceService, IPurchaseInvoiceService purchaseInvoiceService)
        {
            _salesInvoiceService = salesInvoiceService;
            _purchaseInvoiceService = purchaseInvoiceService;
        }

        public async Task<SalesTaxReturnDto> GetVatReturnAsync(int year, int quarter)
        {
            var totalSalesTaxCollected = await _salesInvoiceService.GetSalesTaxReturnForQuarterAsync(year, quarter);
            var totalPurchaseTaxPaid = await _purchaseInvoiceService.GetPurchaseTaxReturnForQuarterAsync(year, quarter);

            return new SalesTaxReturnDto
            {
                Year = year,
                Quarter = quarter,
                TotalSalesTaxCollected = totalSalesTaxCollected,
                TotalPurchaseTaxPaid = totalPurchaseTaxPaid
            };
        }
    }
}
