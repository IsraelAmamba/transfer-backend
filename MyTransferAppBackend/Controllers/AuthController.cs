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
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserAuthentication _iUserAuth;

        public AuthController(IUserAuthentication userService)
        {
            _iUserAuth = userService;
        }

        [HttpPost("generate-token")]
        public async Task<ApiResponse<TokenAuthResponse>> GenerateToken(TokenRequest model)
        {            
            var response = await _iUserAuth.Authenticate(model);
            if (response.responseCode == "00") addRefreshTokenHeaderTokenCookie(response.responseData?.FirstOrDefault()?.RefreshToken);
            return response;
        }
        
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ApiResponse<TokenAuthResponse>> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            var response = await _iUserAuth.RefreshToken(model);
            if (response.responseCode == "00") addRefreshTokenHeaderTokenCookie(response.responseData?.FirstOrDefault()?.RefreshToken);
            return response;

        }

        [HttpPost("revoke-token")]
        public async Task<ApiResponse<TokenRevokeResponse>> RevokeToken([FromBody] RefreshTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];


            return await _iUserAuth.RevokeToken(token);
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _iUserAuth.GetById(id);
            if (user == null) return NotFound();

            return Ok(user.RefreshTokens);
        }


        private void addRefreshTokenHeaderTokenCookie(string token)
        {           
            Response.Headers.Add("RefreshToken", token);
        }

    }
}
