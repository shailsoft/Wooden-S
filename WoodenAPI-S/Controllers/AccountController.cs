using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WoodenAPI_S.Models;
using WoodenAPI_S.Models.CustomeModel;

namespace WoodenAPI_S.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region constructor
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        /// <summary>
        /// created by shailendra
        /// created date 23 apr 2022
        /// </summary>
        /// <param name="modelRequest"></param>
        /// <returns></returns>
        [HttpPost]
       
        [Route("CreateUser")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserModel modelRequest)
        {
            var userExists = await _userManager.FindByEmailAsync(modelRequest.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status403Forbidden, new Response<UserModel> { StatusMessage = "Email Already exist", Status = Models.CustomeModel.StatusCode.Success, Result = null });

            ApplicationUser user = new ApplicationUser
            {
                Email = modelRequest.Email,
                FirstName = modelRequest.FirstName,
                MiddleName = modelRequest.MiddleName,
                LastName = modelRequest.LastName,
                PhoneNumber = modelRequest.PhoneNumber,
                UserName = modelRequest.Email,
                CreatedDate = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, modelRequest.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<UserModel> { StatusMessage = "Successfully Created", Status = Models.CustomeModel.StatusCode.Success, Result = null });

            // Checking roles in database and creating if not exists
            if (!await _roleManager.RoleExistsAsync(modelRequest.Role))
                await _roleManager.CreateAsync(new IdentityRole(modelRequest.Role));
            if (!await _roleManager.RoleExistsAsync(modelRequest.Role))
                await _roleManager.CreateAsync(new IdentityRole(modelRequest.Role));

            // Add role to user
            if (!string.IsNullOrEmpty(modelRequest.Role) && modelRequest.Role == ApplicationUserRoles.Admin)
            {
                await _userManager.AddToRoleAsync(user, ApplicationUserRoles.Admin);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, ApplicationUserRoles.User);
            }
            return Ok(new Response<UserModel> { StatusMessage = "User Creation Successfully", Status = Models.CustomeModel.StatusCode.Success });
        }

     
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserModel userModelLogRequest)
        {
            var user = await _userManager.FindByNameAsync(userModelLogRequest.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, userModelLogRequest.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
      
        [HttpGet]
        [Route("GetStudentList")]
        public IActionResult GetStudentList()
        {
            UserModel userModel = new UserModel()
            {
                FirstName = "Shailendra Kumar",
                LastName = "Bharti",
                Email = "shailendrab@chetu.com"
            };
            return  StatusCode(StatusCodes.Status200OK, new Response<UserModel> { StatusMessage = "Email Already exist", Status = Models.CustomeModel.StatusCode.Success, Result = userModel });
        }

    }
}
