using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notebook.Infrastructure.UnitOfWorks;
using Notebook.Models.Dtos.Requets;
using Notebook.Models.Dtos.Responses;
using Notebook.Models.Users;

namespace Notebook.Api.Controllers
{
    [Route("users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController: BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) : base(unitOfWork)
        {
            _userManager = userManager;
        }

        [HttpPost()]
        public IActionResult CreateUser([FromBody] CreateUserDto user)
        {
            var userModel = new User()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            UnitOfWork.UserRepository.Add(userModel);
            var response =  UnitOfWork.Commit();

            return response.IsCompletedSuccessfully ? Ok(userModel.Id) : BadRequest(response.Exception.Message);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user= await UnitOfWork.UserRepository.GetById(id);
            var userDto = new GetUsersDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id
            };

            return Ok(userDto);

        }

        [HttpGet()]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await UnitOfWork.UserRepository.GetAll();
            var userDtos = new List<GetUsersDto>();
            foreach (var user in users)
            {
                userDtos.Add(new GetUsersDto
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id
                });
            }

            return Ok(userDtos);
        }

        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIden = await _userManager.GetUserAsync(HttpContext.User);

            var user = await UnitOfWork.UserRepository.GetUserAsync(x=>x.IdentityId== new Guid(userIden.Id));
            var userDtos = new GetUsersDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id
            };

            return Ok(userDtos);
        }
    }
}
