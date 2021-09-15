using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyTransferAppBackend.Constants;
using MyTransferAppBackend.DbContexts;
using MyTransferAppBackend.Helpers;
using MyTransferAppBackend.Models.Requests;
using MyTransferAppBackend.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MyTransferAppBackend.Services
{
    public interface IProcessTransfers
    {
        Task<ApiResponse<TransferResponse>> SavingToCurrent(List<TransferRequest> requests);
    }
    public class ProcessTransfers : IProcessTransfers
    {
        private readonly ILogger<ProcessTransfers> logger;
        private readonly DatabaseContext _dbContext;

        public ProcessTransfers(ILogger<ProcessTransfers> logger, DatabaseContext context)
        {
            this.logger = logger;
            _dbContext = context;
        }

        public async Task<ApiResponse<TransferResponse>> SavingToCurrent(List<TransferRequest> requests)
        {
            List<int> transactionId = new List<int>();

            //add strategry to care for the rolebacks
            var strategry = _dbContext.Database.CreateExecutionStrategy();

            await strategry.ExecuteAsync(async () =>
            {
                try
                {
                    //using Transactions
                    using (var t = _dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in requests)
                        {
                            await _dbContext.Transactions.AddAsync(item);
                            var id = await _dbContext.SaveChangesAsync();
                            transactionId.Add(id);
                        }

                        //commit if all success, automatic rollback happens if one or more fails
                        t.Commit();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Error SavingToCurrent", ex);
                }
            });

            //we believe all was fine. Todo check for failure
            return ResponseGenerator<TransferResponse>.GenerateResponse(ResponseCodes.SUCCESS, null, "Transaction has been successfully posted");
        }

    }
}
