using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
    public interface INominalAccountResolutionService
    {
        Task<Guid> ResolveOrCreateNominalAccountAsync(string? nominalName, ApplicationDbContext.NominalAccountType? nominalType);
    }
    public class NominalAccountResolutionService : INominalAccountResolutionService
    {
        private readonly ApplicationDbContext _dbContext;

        public NominalAccountResolutionService(ApplicationDbContext dbContext, LlmService llmService)
        {
            _dbContext = dbContext;
        }
        // Implementation of the method
        // This method resolves or creates a nominal account based on the provided name and type.
        // It checks if the nominal account exists in the database,
        // and if not, it creates a new nominal account with the specified name and type
        public async Task<Guid> ResolveOrCreateNominalAccountAsync(string? nominalName, ApplicationDbContext.NominalAccountType? nominalType)
        {
            if (string.IsNullOrWhiteSpace(nominalName) || nominalType == null)
            {
                throw new ArgumentException("Nominal name and type must be provided.");
            }

            var existingAccount = await _dbContext.NominalAccounts
                .FirstOrDefaultAsync(a => a.Name == nominalName && a.type == nominalType);
            // Check if the nominal account already exists
            if (existingAccount != null)
            {
                return existingAccount.Id; // Return the existing account ID
            }
            if (!nominalType.HasValue)
            {
                throw new ArgumentException("Nominal type must be specified.");
            }
            string newCode = (await GenerateUniqueNominalCodeAsync(nominalType.Value)).ToString();
            // If the nominal account does not exist, create a new one
            if (string.IsNullOrWhiteSpace(newCode))
            {
                throw new InvalidOperationException("Failed to generate a unique code for the nominal account.");
            }
            // Create a new nominal account if it does not exist
            var newAccount = new ApplicationDbContext.NominalAccount
            {
                Name = nominalName,
                type = nominalType.Value,
                Code = Guid.NewGuid().ToString() // Set the required 'Code' property
            };

            _dbContext.NominalAccounts.Add(newAccount);
            await _dbContext.SaveChangesAsync();

            return newAccount.Id; // Return the newly created account ID
        }
        // This method generates a unique code for the nominal account
        private async Task<string> GenerateUniqueNominalCodeAsync(ApplicationDbContext.NominalAccountType nominalType)
        {
            // Generate a unique code based on the nominal type
            string prefix;
            int startRange; // Starting range for nominal codes
            int endRange; // Ending range for nominal codes
            switch (nominalType)
            {
                case ApplicationDbContext.NominalAccountType.Asset:
                    prefix = "A";
                    startRange = 1000;
                    endRange = 1999;
                    break;
                case ApplicationDbContext.NominalAccountType.Liability:
                    prefix = "L";
                    startRange = 2000;
                    endRange = 2999;
                    break;
                case ApplicationDbContext.NominalAccountType.Equity:
                    prefix = "E";
                    startRange = 3000;
                    endRange = 3999;
                    break;
                case ApplicationDbContext.NominalAccountType.Revenue:
                    prefix = "R";
                    startRange = 4000;
                    endRange = 4999;
                    break;
                case ApplicationDbContext.NominalAccountType.Expense:
                    prefix = "X";
                    startRange = 5000;
                    endRange = 5999;
                    break;
                default:
                    prefix = "9"; startRange = 9000; endRange = 9999; break; // Unknown type
            }
            // Find the last code for the given nominal type
            var lastCode = await _dbContext.NominalAccounts
                .Where(a => a.type == nominalType &&
                            a.Code.StartsWith(prefix))
                .Select(a => a.Code)
                .FirstOrDefaultAsync(); // Perform async operation before switching to in-memory evaluation

            int newCodeNum;
            if (lastCode != null)
            {
                // If a last code exists, increment the code number
                if (int.TryParse(lastCode.Substring(prefix.Length), out newCodeNum))
                {
                    newCodeNum++;
                }
                else
                {
                    throw new InvalidOperationException("Failed to parse the last nominal account code.");
                }
            }
            else
            {
                // If no last code exists, start from the beginning of the range
                newCodeNum = startRange;
            }

            string proposedCode = $"{prefix}{newCodeNum:D4}"; // Format the new code with leading zeros
            while (await _dbContext.NominalAccounts.AnyAsync(a => a.Code == proposedCode))
            {
                // If the proposed code already exists, increment the code number until a unique code is found
                newCodeNum++;
                if (newCodeNum > endRange)
                {
                    throw new InvalidOperationException("No available nominal account codes in the specified range.");
                }
                proposedCode = $"{prefix}{newCodeNum:D4}";
            }
            if (string.IsNullOrWhiteSpace(proposedCode))
            {
                throw new InvalidOperationException("Failed to generate a unique code for the nominal account.");
            }
            return proposedCode; // Return the unique code
        }
    }
}