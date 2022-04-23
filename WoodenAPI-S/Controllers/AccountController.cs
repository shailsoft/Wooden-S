using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoodenAPI_S.Models;
using WoodenAPI_S.Models.CustomeModel;

namespace WoodenAPI_S.Controllers
{
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
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<UserModel> { StatusMessage = "Successfully Created", Status = Models.CustomeModel.StatusCode.Success , Result = null });
            else
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
            }
            return Ok(new Response<UserModel> { StatusMessage = "User Creation Successfully", Status = Models.CustomeModel.StatusCode.Success });
        }
    }
}
