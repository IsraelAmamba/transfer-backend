using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Models.Responses
{

    public class ApiResponse<T> where T : class
    {
        public string responseCode { get; set; }
        public List<T> responseData { get; set; }
        public string responseMsg { get; set; }
    }
}
