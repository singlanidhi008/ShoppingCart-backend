using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartBusinessLayer.Services;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MvcCoreAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountRepository _accountRepository;
        
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _accountRepository = accountRepository;
        }
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users;
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDto model)
        {

            var result = await _accountRepository.Login(model);
            if (result != null)
            {
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                //return Unauthorized();
            }
            return Unauthorized();
        }
        //public async Task<IActionResult> Login([FromBody] LoginModelDto model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        var userRoles = await _userManager.GetRolesAsync(user);

        //        var authClaims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.UserName),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        };

        //        foreach (var userRole in userRoles)
        //        {
        //            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //        }

        //        var token = GenerateToken(authClaims);



        //        return Ok(new JwtSecurityTokenHandler().WriteToken(token));


        //    }

        //    return Unauthorized();
        //}
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            //if (userExists != null)
            //    return Ok("User already Exist");

            if(userExists!=null)
            {
                return Conflict();
            }

            User user = new()
            {
                Id=Guid.NewGuid().ToString(),
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
     
            };
           
            var result = await _userManager.CreateAsync(user, model.ConfirmPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user,"User");
                return Ok();
            }
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(errors);
        }
        //un used
        private JwtSecurityToken GenerateToken(List<Claim> authClaims)
        {
            if (authClaims == null)
            {
                throw new ArgumentNullException(nameof(authClaims));
            }
            var jwtSettings = _configuration.GetSection("Jwt");

            var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

    }
}
