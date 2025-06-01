using ERP.Model;

namespace ERP.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AccountRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApplicationDbContext.BankAccount> addAccount(ApplicationDbContext.BankAccount bankAccount)
        {
            var existingAccount = _dbContext.BankAccounts.FirstOrDefault(x => x.Id == bankAccount.Id);
            if (existingAccount != null)
            {
                throw new InvalidOperationException("Account already exists");
            }
            _dbContext.BankAccounts.Add(bankAccount);
            await _dbContext.SaveChangesAsync();
            return bankAccount;
        }
        public Task<ApplicationDbContext.BankAccount> GetAccounts(Guid Id)
        {
            var account = _dbContext.BankAccounts.FirstOrDefault(x => x.Id == Id);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found");
            }
            return Task.FromResult(account);
        }
        public async Task<ApplicationDbContext.BankAccount> DeleteAccount (Guid Id)
        {
            var account = await GetAccounts(Id);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found");
            }
            _dbContext.BankAccounts.Remove(account);
            await _dbContext.SaveChangesAsync();
            return account;
        }
        public async Task<ApplicationDbContext.BankAccount> UpdateAccount(ApplicationDbContext.BankAccount bankAccount)
        {
            var existingAccount = await GetAccounts(bankAccount.Id);
            if (existingAccount == null)
            {
                throw new InvalidOperationException("Account not found");
            }
            existingAccount.AccountNumber = bankAccount.AccountNumber;
            existingAccount.AccountName = bankAccount.AccountName;
            existingAccount.BankName = bankAccount.BankName;
            existingAccount.Balance = bankAccount.Balance;
            _dbContext.BankAccounts.Update(existingAccount);
            await _dbContext.SaveChangesAsync();
            return existingAccount;
        }
        public void ReconcileAccount(Guid accountId, decimal debits, decimal credits)
        {
            var account = _dbContext.BankAccounts.FirstOrDefault(x => x.Id == accountId) ?? throw new InvalidOperationException("Account not found");
            decimal netEffect = debits - credits;
            account.Balance += netEffect;

            _dbContext.BankAccounts.Update(account);
            _dbContext.SaveChanges();
        }
    }

    public interface IAccountRepository
    {
        Task<ApplicationDbContext.BankAccount> UpdateAccount(ApplicationDbContext.BankAccount bankAccount);
        Task<ApplicationDbContext.BankAccount> DeleteAccount (Guid Id);
        Task<ApplicationDbContext.BankAccount> GetAccounts(Guid Id);
        Task<ApplicationDbContext.BankAccount> addAccount(ApplicationDbContext.BankAccount bankAccount);
        void ReconcileAccount(Guid accountId, decimal debits, decimal credits);
    }
}