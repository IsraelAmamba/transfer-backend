using MyTransferAppBackend.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using MyTransferAppBackend.Models.Responses;
using MyTransferAppBackend.Constants;
using MyTransferAppBackend.Models.Requests;
using System.Security.Cryptography;
using MyTransferAppBackend.DbContexts;
using MyTransferAppBackend.Helpers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Services
{
   
    public interface IUserAuthentication
    {
        User GetById(int id);
        Task<ApiResponse<TokenAuthResponse>> RefreshToken(RefreshTokenRequest model);
        Task<ApiResponse<TokenRevokeResponse>> RevokeToken(string token);
        Task<ApiResponse<TokenAuthResponse>> Authenticate(TokenRequest model);      
    }

    public class UserAuthentication : IUserAuthentication
    {
        

        private readonly AppSettings _appSettings;
        private readonly DatabaseContext _context;
        ILogger<UserAuthentication> _logger;

        public UserAuthentication(IOptions<AppSettings> appSettings, DatabaseContext context, ILogger<UserAuthentication> logger)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _logger = logger;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public async Task<ApiResponse<TokenAuthResponse>> Authenticate(TokenRequest model)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

                // return if user not found
                if (user == null)
                    return ResponseGenerator<TokenAuthResponse>.GenerateResponse(ResponseCodes.FAILURE, null, "Invalid username or password");

                // authentication successful so generate jwt and refresh tokens
                var jwtToken = generateJwtToken(user);
                var refreshToken = generateRefreshToken();

                // save refresh token
                user.RefreshTokens.Add(refreshToken);
                _context.Update(user);
                await _context.SaveChangesAsync();

                return ResponseGenerator<TokenAuthResponse>
                    .GenerateResponse(ResponseCodes.SUCCESS, new List<TokenAuthResponse> { new TokenAuthResponse(user, jwtToken, refreshToken.Token) }, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in authenticating user");
                return ResponseGenerator<TokenAuthResponse>.GenerateResponse(ResponseCodes.FAILURE, null, "Error generating token");
            }
        }

        private string generateJwtToken(User user)
        {
            // generate token that is valid for days specified in appsettings.json
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.JwtConfig.expirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ApiResponse<TokenAuthResponse>> RefreshToken(RefreshTokenRequest model)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == model.Token));

                // return if no user is found
                if (user == null)
                    return ResponseGenerator<TokenAuthResponse>.GenerateResponse(ResponseCodes.TOKEN_INVALID, null, "Token is invalid");

                var refreshToken = user.RefreshTokens.Single(x => x.Token == model.Token);

                // return  if token is no longer active
                if (!refreshToken.IsActive)
                    return ResponseGenerator<TokenAuthResponse>.GenerateResponse(ResponseCodes.TOKEN_EXPIRED, null, "No user is found with token");

                // replace old refresh token with a new one and save
                var newRefreshToken = generateRefreshToken();
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.ReplacedByToken = newRefreshToken.Token;
                user.RefreshTokens.Add(newRefreshToken);
                _context.Update(user);
                await _context.SaveChangesAsync();

                // generate new jwt
                var jwtToken = generateJwtToken(user);

                return ResponseGenerator<TokenAuthResponse>
                        .GenerateResponse(ResponseCodes.SUCCESS, new List<TokenAuthResponse> { new TokenAuthResponse(user, jwtToken, newRefreshToken.Token) }, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshToken error");
                return ResponseGenerator<TokenAuthResponse>.GenerateResponse(ResponseCodes.FAILURE, null);
            }
        }

        public async Task<ApiResponse<TokenRevokeResponse>> RevokeToken(string token)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

                // return  if no user found with token
                if (user == null)
                    return ResponseGenerator<TokenRevokeResponse>.GenerateResponse(ResponseCodes.FAILURE, null, "No user with token");

                var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

                // return  if token is not active
                if (!refreshToken.IsActive)
                    return ResponseGenerator<TokenRevokeResponse>.GenerateResponse(ResponseCodes.TOKEN_INACTIVE, null);

                // revoke token and save
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.RevokedByIp = null;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return ResponseGenerator<TokenRevokeResponse>.GenerateResponse(ResponseCodes.SUCCESS, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RevokeToken");
                return ResponseGenerator<TokenRevokeResponse>.GenerateResponse(ResponseCodes.FAILURE, null);
            }
        }

       
        private RefreshToken generateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = null
                };
            }
        }
    }
}
