using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartBusinessLayer.Services
{
    public class AccountRepository:IAccountRepository
    {
        private readonly UserManager<User> _userManager;     
        private readonly IConfiguration _config;
      
        public AccountRepository(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
            
        }
        public async Task<LoginREsModel> Login(LoginModelDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {

                return new LoginREsModel()
                {
                    IsSuccess = false
                };
            }
            var PasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!PasswordValid)
            {
                return new LoginREsModel()
                {
                    IsSuccess = false
                };
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var tokenHendeler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._config.GetSection("Jwt")["SecretKey"]);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = System.DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHendeler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHendeler.WriteToken(token);
            return new LoginREsModel()
            {
                 id=user.Id,
                IsSuccess = true,
                Token = encryptedToken,
                Username = user.UserName,
                Role = userRoles[0],
                Image=user.Image
            };
        }

      


    }

}
    

