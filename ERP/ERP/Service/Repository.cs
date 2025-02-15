using ERP.Model;

namespace ERP.Services
{
    public class PurchaseInvoiceRepository : IRepository<PurchaseInvoice>
    {
        private readonly ApplicationDbContext _context;
        public PurchaseInvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public PurchaseInvoice Get(int id)
        {
            return _context.PurchaseInvoices.Find(id);
        }
    }
}