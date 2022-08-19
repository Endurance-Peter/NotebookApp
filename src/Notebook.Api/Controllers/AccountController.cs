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

                var token = GenerateToken(fetchUser);

                return Ok(new RegistrationResponse { IsSuccess = true, Token = token });
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

                    var token = GenerateToken(user);

                    return Ok(new RegistrationResponse { IsSuccess = true, Token = token });
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

        private string GenerateToken(IdentityUser user)
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
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
