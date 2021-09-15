using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Models
{
    public class Transactions
    {
        public int id { get; set; }
        public DateTime dateCreated { get; set; } = DateTime.Now;
        public string DestinationInstitutionCode { get; set; }
        public string senderBankCode { get; set; }
        public string trnStatus { get; set; }
        public string narration { get; set; }
        [Required]
        public int initiatorId { get; set; }
        [Required]
        public double trnAmount { get; set; }
        [Required]
        public string senderAccountNo { get; set; }
        [Required]
        public string beneficiaryAccountNo { get; set; }
    }
}
