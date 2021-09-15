using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Constants
{
    
    public enum ResponseCodes
    {
        /// <summary>
        /// Success
        /// </summary>
        SUCCESS = 00,

        FAILURE = 11,

        

        /// <summary>
        /// when the sender account number is invalid
        /// </summary>

        INVALID_SENDER_ACCOUNT = 12,

        /// <summary>
        /// when the recipient account is invalid
        /// </summary>
        INVALID_RECIPIENT = 13,

        TOKEN_INVALID = 14,
        TOKEN_EXPIRED = 15,
        TOKEN_INACTIVE = 15,






    }
}
