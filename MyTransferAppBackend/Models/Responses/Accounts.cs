using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Models.Responses
{
    public class Accounts
    {
        public int customerId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public double LedgerBalance { get; set; }
        public double EffectiveBalance { get; set; }
        public char AccountType { get; set; }
    }
}
