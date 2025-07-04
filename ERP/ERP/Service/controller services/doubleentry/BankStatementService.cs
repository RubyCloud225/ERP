using ERP.Model;
using static ERP.Model.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Service
{
    public interface IBankStatementService
    {
        Task<BankStatement> ProcessBankStatementAsync(ParsedBankStatementDto parsedBankStatement, Guid? userId);
    }

    public class BankStatementService : IBankStatementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILlmService _llmService;
        private readonly IAccountingService _accountingService;
        private readonly INominalAccountResolutionService _nominalAccountResolutionService;

        public BankStatementService(ApplicationDbContext dbContext, ILlmService llmService, IAccountingService accountingService, INominalAccountResolutionService nominalAccountResolutionService)
        {
            _dbContext = dbContext;
            _llmService = llmService;
            _accountingService = accountingService;
            _nominalAccountResolutionService = nominalAccountResolutionService;
        }

        public async Task<BankStatement> ProcessBankStatementAsync(ParsedBankStatementDto parsedBankStatement, Guid? userId)
        {
            if (parsedBankStatement == null)
            {
                throw new ArgumentNullException(nameof(parsedBankStatement), "Parsed bank statement cannot be null");
            }

            var bankStatement = new BankStatement
            {
                Id = Guid.NewGuid(),
                BlobName = parsedBankStatement.BlobName,
                StatementStartDate = parsedBankStatement.StatementStartDate,
                StatementEndDate = parsedBankStatement.StatementEndDate,
                OpeningBalance = parsedBankStatement.OpeningBalance,
                ClosingBalance = parsedBankStatement.ClosingBalance,
                StatementNumber = parsedBankStatement.StatementNumber,
                UserId = userId,
                Transactions = new List<BankTransaction>()
            };

            _dbContext.BankStatements.Add(bankStatement);
            await _dbContext.SaveChangesAsync();

            foreach (var transactionDto in parsedBankStatement.Transactions)
            {
                // Call LLM service to get nominal recommendation for each transaction
                string prompt = $"You are an expert accounting AI. Please recommend the appropriate nominal account for the following bank transaction:\n" +
                                $"Description: {transactionDto.Description}\n" +
                                $"Date: {transactionDto.TransactionDate:yyyy-MM-dd}\n" +
                                $"Amount: {transactionDto.Amount:C}\n" +
                                $"Transaction Type: {transactionDto.TransactionType}\n";

                string llmResponse = await _llmService.GenerateResponseAsync(prompt);

                // For simplicity, assume the LLM response is the nominal account name
                string recommendedNominalName = llmResponse.Trim();

                // Resolve or create nominal account
                Guid nominalAccountId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(recommendedNominalName, null);

                var bankTransaction = new BankTransaction
                {
                    Id = Guid.NewGuid(),
                    TransactionNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    TransactionDate = transactionDto.TransactionDate,
                    Amount = transactionDto.Amount,
                    TransactionType = transactionDto.TransactionType,
                    Description = transactionDto.Description,
                    BankStatementId = bankStatement.Id,
                    NominalAccountId = nominalAccountId
                };

                _dbContext.BankTransactions.Add(bankTransaction);
                await _dbContext.SaveChangesAsync();

                // Create accounting entry for the bank transaction
                await _accountingService.CreateAccountingForBankTransactionAsync(bankTransaction, nominalAccountId, userId);
            }

            return bankStatement;
        }
    }
}
