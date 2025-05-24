using Azure.Storage.Sas;
using ERP.Model;
using ERP.Repository;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static ERP.Model.ApplicationDbContext;

namespace ERP.Service
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDocumentProcessor _documentProcessor;
        private readonly ILlmService _llmService;
        private readonly ApplicationDbContext _dbContext;
        public BankService(IBankRepository bankRepository, ApplicationDbContext dbContext ,IDocumentProcessor documentProcessor, ILlmService llmService, IAccountRepository accountRepository)
        {
            _bankRepository = bankRepository;
            _accountRepository = accountRepository;
            _documentProcessor = documentProcessor;
            _llmService = llmService;
            _dbContext = dbContext;
        }
        // Add methods to handle Bankstatements between LLM and Document Processor
        public async Task ProcessBankStatementAsync(IFormFile document, BankAccount bankAccount, BankReceipt bankReceipt, BankPayment bankPayment, BankStatement bankStatement)
        {
            using (var stream = document.OpenReadStream())
            {
                string fileName = bankStatement.BlobName;
                DateTime statementDate = bankStatement.StatementDate;
                decimal statementBalance = bankStatement.Balance;
                string description = "Bank Statement for " + bankAccount.AccountNumber;
                // step 1: generate prompt for the llm
                bankStatement.BlobName = fileName;
                bankStatement.StatementDate = statementDate;
                bankStatement.Balance = statementBalance;
                bankAccount.AccountNumber = bankAccount.AccountNumber;
                bankAccount.AccountName = bankAccount.AccountName;
            }
            // step 1: generate prompt for the llm
            var bankStatementInstance = new BankStatement
            {
                StatementDate = bankStatement.StatementDate,
                Balance = bankStatement.Balance,
                BlobName = bankStatement.BlobName
            };
            var bankAccountInstance = new BankAccount
            {
                AccountNumber = bankAccount.AccountNumber,
                AccountName = bankAccount.AccountName,
                BankName = bankAccount.BankName // Ensure BankName is set
            };
            var bankReceiptInstance = new BankReceipt
            {
                Payer = bankReceipt.Payer,
                Amount = bankReceipt.Amount,
                ReceiptDate = bankReceipt.ReceiptDate
            };
            var bankPaymentInstance = new BankPayment
            {
                Payee = bankPayment.Payee,
                Amount = bankPayment.Amount,
                PaymentDate = bankPayment.PaymentDate
            };
            string prompt = await _documentProcessor.BankStatementProcessorAsync(bankStatementInstance, bankAccountInstance, bankReceiptInstance, bankPaymentInstance);
            // step 2: process the document using the llm
            string llmResponse = await _llmService.GenerateResponseAsync(prompt);
            var accounts = ParseLlmResponse(llmResponse);
            // step 3: process the response from the llm
            await _bankRepository.AddBankStatement(bankStatementInstance);
        }
        private BankAccount ParseLlmResponse(string llmResponse)
        {
            // Logic to parse the LLM response and extract bank account details
            // This is a placeholder implementation
            // In a real implementation, you would parse the response string to extract the relevant information
            var lines = llmResponse.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string accountNumber = string.Empty;
            string accountName = string.Empty;
            string bankName = string.Empty;
            foreach (var line in lines)
            {
                if (line.StartsWith("Account Number:"))
                {
                    accountNumber = line.Substring("Account Number:".Length).Trim();
                }
                else if (line.StartsWith("Account Name:"))
                {
                    accountName = line.Substring("Account Name:".Length).Trim();
                }
                else if (line.StartsWith("Bank Name:"))
                {
                    bankName = line.Substring("Bank Name:".Length).Trim();
                }
            }
            
            return new BankAccount
            {
                AccountNumber = accountNumber,
                AccountName = accountName,
                BankName = bankName
            };
        }
    }
    public interface IBankService
    {
        Task ProcessBankStatementAsync(IFormFile document, BankAccount bankAccount, BankReceipt bankReceipt, BankPayment bankPayment, BankStatement bankStatement);
    }
}