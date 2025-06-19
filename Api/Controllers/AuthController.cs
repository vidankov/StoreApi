using Api.Common;
using Api.Data;
using Api.Model;
using Api.ModelDto;
using Api.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Controllers
{
    public class AuthController : StoreController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JwtTokenGenerator jwtTokenGenerator;

        public AuthController(
            AppDbContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            JwtTokenGenerator jwtTokenGenerator)
            : base(dbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto registerRequestDto)
        {
            if (registerRequestDto is null)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Некорректная модель запроса" }
                });
            }

            var userFromDb = await dbContext
                .AppUsers
                .FirstOrDefaultAsync(u =>
                    u.UserName.ToLower() == registerRequestDto.UserName.ToLower());

            if (userFromDb is not null)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Пользователь уже существует." }
                });
            }

            var newAppUser = new AppUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                NormalizedEmail = registerRequestDto.Email.ToUpper()
            };

            var result = await userManager.CreateAsync(
                newAppUser, registerRequestDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Ошибка регистрации." }
                });
            }

            var newRoleAppUser = registerRequestDto.Role.Equals(
                SharedData.Roles.Admin, StringComparison.OrdinalIgnoreCase)
                ? SharedData.Roles.Admin : SharedData.Roles.Consumer;

            await userManager.AddToRoleAsync(newAppUser, newRoleAppUser);

            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = $"Пользователь {newAppUser.UserName} успешно зарегистрирован!"
            });
        }

        [HttpPost]
        public async Task<ActionResult<ResponseServer>> Login(
            [FromBody] LoginRequestDto loginRequestDto)
        {
            var userFromDb = await dbContext
                .AppUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDto.Email.ToLower());

            if (userFromDb is null ||
                !await userManager.CheckPasswordAsync(userFromDb, loginRequestDto.Password))
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Неверная пара логин/пароль!" }
                });
            }

            var roles = await userManager.GetRolesAsync(userFromDb);
            var token = jwtTokenGenerator.GenerateJwtToken(userFromDb, roles);

            return Ok(new ResponseServer 
            {
                StatusCode = HttpStatusCode.OK,
                Result = new LoginResponseDto
                {
                    Email = userFromDb.Email,
                    Token = token
                }
            });
        }
    }
}
