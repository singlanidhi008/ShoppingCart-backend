using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartBusinessLayer.Services;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System.Data;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly AuthService _Service;
        private readonly IGenericRepository<User> _genericRepository;

        public AuthController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IAccountRepository accountRepository,
            IWebHostEnvironment webHostEnvironment, AuthService Service, IGenericRepository<User> genericRepository)
        {
            _Service = Service;
            _genericRepository = genericRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _accountRepository = accountRepository;
            _webHostEnvironment = webHostEnvironment;
          
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
        [HttpGet("GetUSerId/{id}")]
        //[Route("{id:guid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserId([FromRoute] string id)
        {
            var result = await _genericRepository.GetById(id);
            if (result == null)
            {
                return BadRequest("iD does not Exist");
            }

            return Ok(result);

        }
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfile updateProfile, [FromRoute] string id)
        {
            if (id == null)
            {
                return BadRequest("Invalid id provided.");
            }

            var result = await _Service.UpdateModel(updateProfile, id);
            if (result == null)
            {
                return BadRequest("User not Updated");
            }

            return Ok(result);

        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] UserRegistrationDto model)
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
            if (model.Image.Length > 0)
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath + "\\uploads\\");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = model.Image.FileName;
                string filePath = Path.Combine(path, filename);


                using (FileStream filestream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(filestream);
                    user.Image = "https://localhost:44357/uploads/" + filename;
                }
            }

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
