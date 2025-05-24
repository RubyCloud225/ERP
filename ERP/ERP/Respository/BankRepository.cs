using ERP.Model;

namespace ERP.Repository
{
    public class BankRepository : IBankRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BankRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApplicationDbContext.BankStatement> AddBankStatement(ApplicationDbContext.BankStatement bankStatement)
        {
            var existingStatement = _dbContext.BankStatements.FirstOrDefault(x => x.Id == bankStatement.Id);
            if (existingStatement != null)
            {
                throw new InvalidOperationException("Bank statement already exists");
            }
            _dbContext.BankStatements.Add(bankStatement);
            await _dbContext.SaveChangesAsync();
            return bankStatement;
        }
        public async Task<ApplicationDbContext.BankPayment> AddBankPayment(ApplicationDbContext.BankPayment bankPayment)
        {
            var existingPayment = _dbContext.BankPayments.FirstOrDefault(x => x.Id == bankPayment.Id);
            if (existingPayment != null)
            {
                throw new InvalidOperationException("Bank payment already exists");
            }
            _dbContext.BankPayments.Add(bankPayment);
            await _dbContext.SaveChangesAsync();
            return bankPayment;
        }
        public async Task<ApplicationDbContext.BankReceipt> AddBankReceipt(ApplicationDbContext.BankReceipt bankReceipt)
        {
            var existingReceipt = _dbContext.BankReceipts.FirstOrDefault(x => x.Id == bankReceipt.Id);
            if (existingReceipt != null)
            {
                throw new InvalidOperationException("Bank receipt already exists");
            }
            _dbContext.BankReceipts.Add(bankReceipt);
            await _dbContext.SaveChangesAsync();
            return bankReceipt;
        }
        public Task<ApplicationDbContext.BankStatement> GetBankStatement(int Id)
        {
            var statement = _dbContext.BankStatements.FirstOrDefault(x => x.Id == Id);
            if (statement == null)
            {
                throw new InvalidOperationException("Bank statement not found");
            }
            return Task.FromResult(statement);
        }
        public Task<ApplicationDbContext.BankPayment> GetBankPayment(int Id)
        {
            var payment = _dbContext.BankPayments.FirstOrDefault(x => x.Id == Id);
            if (payment == null)
            {
                throw new InvalidOperationException("Bank payment not found");
            }
            return Task.FromResult(payment);
        }
        public Task<ApplicationDbContext.BankReceipt> GetBankReceipt(int Id) {
            var receipt = _dbContext.BankReceipts.FirstOrDefault(x => x.Id == Id);
            if (receipt == null)
            {
                throw new InvalidOperationException("Bank receipt not found");
            }
            return Task.FromResult(receipt);
        }
        public async Task<ApplicationDbContext.BankStatement> DeleteBankStatement(int Id)
        {
            var statement = await GetBankStatement(Id);
            if (statement == null)
            {
                throw new InvalidOperationException("Bank statement not found");
            }
            _dbContext.BankStatements.Remove(statement);
            await _dbContext.SaveChangesAsync();
            return statement;
        }
        public async Task<ApplicationDbContext.BankPayment> DeleteBankPayment(int Id)
        {
            var payment = await GetBankPayment(Id);
            if (payment == null)
            {
                throw new InvalidOperationException("Bank payment not found");
            }
            _dbContext.BankPayments.Remove(payment);
            await _dbContext.SaveChangesAsync();
            return payment;
        }
        public async Task<ApplicationDbContext.BankReceipt> DeleteBankReceipt(int Id)
        {
            var receipt = await GetBankReceipt(Id);
            if (receipt == null)
            {
                throw new InvalidOperationException("Bank receipt not found");
            }
            _dbContext.BankReceipts.Remove(receipt);
            await _dbContext.SaveChangesAsync();
            return receipt;
        }
        public async Task<ApplicationDbContext.BankStatement> UpdateBankStatement(ApplicationDbContext.BankStatement bankStatement)
        {
            var existingStatement = await GetBankStatement(bankStatement.Id);
            if (existingStatement == null)
            {
                throw new InvalidOperationException("Bank statement not found");
            }
            existingStatement.BlobName = bankStatement.BlobName;
            existingStatement.StatementDate = bankStatement.StatementDate;
            existingStatement.Balance = bankStatement.Balance;
            _dbContext.BankStatements.Update(existingStatement);
            await _dbContext.SaveChangesAsync();
            return existingStatement;
        }
        public async Task<ApplicationDbContext.BankPayment> UpdateBankPayment(ApplicationDbContext.BankPayment bankPayment)
        {
            var existingPayment = await GetBankPayment(bankPayment.Id);
            if (existingPayment == null)
            {
                throw new InvalidOperationException("Bank payment not found");
            }
            existingPayment.Payee = bankPayment.Payee;
            existingPayment.Amount = bankPayment.Amount;
            existingPayment.PaymentDate = bankPayment.PaymentDate;
            _dbContext.BankPayments.Update(existingPayment);
            await _dbContext.SaveChangesAsync();
            return existingPayment;
        }
        public async Task<ApplicationDbContext.BankReceipt> UpdateBankReceipt(ApplicationDbContext.BankReceipt bankReceipt)
        {
            var existingReceipt = await GetBankReceipt(bankReceipt.Id);
            if (existingReceipt == null)
            {
                throw new InvalidOperationException("Bank receipt not found");
            }
            existingReceipt.Payer = bankReceipt.Payer;
            existingReceipt.Amount = bankReceipt.Amount;
            existingReceipt.ReceiptDate = bankReceipt.ReceiptDate;
            _dbContext.BankReceipts.Update(existingReceipt);
            await _dbContext.SaveChangesAsync();
            return existingReceipt;
        }
    }
    public interface IBankRepository
    {
        Task<ApplicationDbContext.BankStatement> UpdateBankStatement(ApplicationDbContext.BankStatement bankStatement);
        Task<ApplicationDbContext.BankPayment> UpdateBankPayment(ApplicationDbContext.BankPayment bankPayment);
        Task<ApplicationDbContext.BankReceipt> UpdateBankReceipt(ApplicationDbContext.BankReceipt bankReceipt);
        Task<ApplicationDbContext.BankStatement> DeleteBankStatement(int Id);
        Task<ApplicationDbContext.BankPayment> DeleteBankPayment(int Id);
        Task<ApplicationDbContext.BankReceipt> DeleteBankReceipt(int Id);
        Task<ApplicationDbContext.BankStatement> AddBankStatement(ApplicationDbContext.BankStatement bankStatement);
        Task<ApplicationDbContext.BankPayment> AddBankPayment(ApplicationDbContext.BankPayment bankPayment);
        Task<ApplicationDbContext.BankReceipt> AddBankReceipt(ApplicationDbContext.BankReceipt bankReceipt);
        Task<ApplicationDbContext.BankStatement> GetBankStatement(int Id);
        Task<ApplicationDbContext.BankPayment> GetBankPayment(int Id);
        Task<ApplicationDbContext.BankReceipt> GetBankReceipt(int Id);
    }
}