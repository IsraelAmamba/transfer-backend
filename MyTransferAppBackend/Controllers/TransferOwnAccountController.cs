using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyTransferAppBackend.Models.Requests;
using MyTransferAppBackend.Models.Responses;
using MyTransferAppBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Controllers
{
    [Route("api/transfer-own")]
    [ApiController]
    public class TransferOwnAccountController : ControllerBase
    {
        private readonly IProcessTransfers processTransfers;

        public TransferOwnAccountController(IProcessTransfers processTransfers)
        {
            this.processTransfers = processTransfers;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SavingToCurrent")]
        public async Task<ApiResponse<TransferResponse>> SavingToCurrent(List<TransferRequest> model)
        {
            return await processTransfers.SavingToCurrent(model);

        }
    }
}
