using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Models.Responses
{
    public class TokenRevokeResponse
    {
        public DateTime RevoteTime { get; set; } = DateTime.Now;
    }
}
