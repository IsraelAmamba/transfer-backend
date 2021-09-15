using Microsoft.Extensions.Options;
using MyTransferAppBackend.Constants;
using MyTransferAppBackend.Helpers;
using MyTransferAppBackend.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTransferAppBackend.Services
{
    public interface IAccountService
    {
        ApiResponse<Accounts> GetAccounts(string customerId);
    }
    public class AccountService : IAccountService
    {
        private readonly AppSettings _options;

        public AccountService(IOptions<AppSettings> options)
        {
            _options = options.Value;
        }

        public ApiResponse<Accounts> GetAccounts(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return ResponseGenerator<Accounts>.GenerateResponse(ResponseCodes.FAILURE, null, "customerId cannot be empty"); 

            var data = _options.AccountDetailsMock.Where(x => x.customerId == int.Parse(customerId)).ToList();

            return data.Count > 0 ?
                ResponseGenerator<Accounts>.GenerateResponse(ResponseCodes.SUCCESS, data) :  
                ResponseGenerator<Accounts>.GenerateResponse(ResponseCodes.FAILURE, null);
        }
    }
}
