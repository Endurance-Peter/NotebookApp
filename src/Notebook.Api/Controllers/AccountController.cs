using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notebook.Api.Authorizations;
using Notebook.Api.Authorizations.Authentication;
using Notebook.Infrastructure.UnitOfWorks;
using Notebook.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Notebook.Api.Controllers
{
    [Route("account")]
    public class AccountController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSecrets _jwtSecrets;
        private readonly TokenValidationParameters _validationParameters;
        public AccountController(IUnitOfWork unitOfWork, 
                                 UserManager<IdentityUser> userManager,
                                 IOptions<JwtSecrets> options) : base(unitOfWork) 
        {
            _userManager = userManager;
            _jwtSecrets = options.Value;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (ModelState.IsValid)
            {
                var fetchUser = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (fetchUser == null) return BadRequest(new RegistrationResponse { IsSuccess = false, Erros = new List<string> { $"Email: {loginRequest.Email} does not exist" } });
                var isConfirmePassword = await _userManager.CheckPasswordAsync(fetchUser, loginRequest.Password);
                if (!isConfirmePassword) return BadRequest(new RegistrationResponse { IsSuccess = false, Erros = new List<string> { $"Unauthorized user" } });

                var token = await GenerateToken(fetchUser);

                return Ok(new RegistrationResponse { IsSuccess = true, Token = token.Token, RefreshToken = token.RefreshToken });
            }
            else
            {
                return BadRequest(new RegistrationResponse { IsSuccess = false, Erros = new List<string> { "Invalid parameters" } });
            }
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest registration)
        {
            if(ModelState.IsValid)
            {
                var fetchUser=await _userManager.FindByEmailAsync(registration.Email);
                if (fetchUser != null) return BadRequest(new RegistrationResponse { IsSuccess = false, Erros = new List<string> { $"Email: {registration.Email} already exist" } });

                var user = new IdentityUser
                {
                    Email = registration.Email,
                    UserName = registration.Email,
                    EmailConfirmed= true
                };

                var created= await _userManager.CreateAsync(user, registration.Password);

                if (created.Succeeded)
                {
                    var userModel = new User
                    {
                        Email = registration.Email,
                        FirstName = registration.FirstName,
                        LastName = registration.LastName,
                        IdentityId = new Guid(user.Id)
                    };
                    UnitOfWork.UserRepository.Add(userModel);
                    await UnitOfWork.Commit();

                    var token = await GenerateToken(user);

                    return Ok(new RegistrationResponse { IsSuccess = true, Token = token.Token, RefreshToken=token.RefreshToken });
                }
                else
                {
                    return BadRequest(new RegistrationResponse { IsSuccess = false, Erros = created.Errors.Select(x=>x.Description).ToList()});
                }
            }
            else
            {
                return BadRequest(new RegistrationResponse { IsSuccess = false, Erros = new List<string> { "Invalid parameters" } });
            }

        }

        [HttpPost("RefereshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            if (ModelState.IsValid)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new RegistrationResponse { IsSuccess = false, Erros = new List<string> { "Invalid parameters" } });
            }
        }

        private async Task<AuthResponse> VerifyRefreshToken(RefreshRequest refreshRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(refreshRequest.Token, _validationParameters, out var validatedToken);

                if(validatedToken is JwtSecurityToken validToken)
                {
                    var result = validToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature);

                    if (!result) return null;
                }

                var expiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

                var expirydate= ConvertToDateTime(expiryDate);

                return new AuthResponse();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DateTime ConvertToDateTime(long value)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var result= dateTime.AddSeconds(value).ToUniversalTime();

            return result;
        }

        private async Task<RefreshData> GenerateToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtSecrets.Secrets);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.Add(_jwtSecrets.ExpiryTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            //refresh token

            var refreshToken = new RefreshToken
            {
                ExpiryDate = DateTime.UtcNow.AddDays(90),
                IsRevoked = false,
                IsUsed = false,
                JwtTokenId = securityToken.Id,
                OwnerId = new Guid(user.Id),
                SoftDelete = false,
                Token = $"{RandomStringGenerator(30)}_{Guid.NewGuid()}",
            };

            UnitOfWork.RefreshTokenRepository.Add(refreshToken);
            await UnitOfWork.Commit();

            var tokenDto = new RefreshData { Token=token, RefreshToken=refreshToken.Token };


            return tokenDto;
        }

        private string RandomStringGenerator(int lenght)
        {
            var rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var values = Enumerable.Repeat(chars, lenght);
            var results= values.Select(s => s[rnd.Next(s.Length)]).ToArray();

            return new String(results);
        }

        //private string GenerateToken(IdentityUser identityUser)
        //{
        //    var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        //    var key = Encoding.ASCII.GetBytes(_jwtSecrets.Secrets);

        //    var tokenDescriptor = new SecurityTokenDescriptor()
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim("Id", identityUser.Id),
        //            new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
        //            new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(3),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);

        //    return jwtSecurityTokenHandler.WriteToken(token);
        //}
    }
}
