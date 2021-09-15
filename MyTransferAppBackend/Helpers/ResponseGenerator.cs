using MyTransferAppBackend.Constants;
using MyTransferAppBackend.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Helpers
{
    public static class ResponseGenerator<T> where T : class
    {
        public static ApiResponse<T> GenerateResponse(ResponseCodes code, List<T> payload, string message = "")
        {
            return new ApiResponse<T>
            {
                responseCode = ((int)code).ToString().Length == 1 ? $"0{((int)code).ToString()}" : ((int)code).ToString(),
                responseData = payload,
                responseMsg = string.IsNullOrEmpty(message) ? code.ToString() : message
            };
        }
    }
}
