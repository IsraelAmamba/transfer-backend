using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Models.Responses
{
    public class TransferResponse
    {
        public List<int> transactionIds { get; set; }
        public string narration { get; set; }
    }
}
