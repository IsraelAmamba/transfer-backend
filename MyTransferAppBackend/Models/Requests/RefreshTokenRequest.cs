using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Models.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}

