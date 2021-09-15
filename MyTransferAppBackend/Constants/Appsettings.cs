using MyTransferAppBackend.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Constants
{
    public class AppSettings
    {
        public JwtConfig JwtConfig { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public List<Accounts> AccountDetailsMock { get; set; }
    }
    
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int expirationInMinutes { get; set; }
    }
}
