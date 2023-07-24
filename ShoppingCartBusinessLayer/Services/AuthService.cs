using Microsoft.AspNetCore.Hosting;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartBusinessLayer.Services
{
    public class AuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGenericRepository<User> _genericRepository;
            public AuthService(IAccountRepository accountRepository,IWebHostEnvironment webHostEnvironment,IGenericRepository<User> genericRepository)
            {
            _accountRepository = accountRepository;
            _webHostEnvironment = webHostEnvironment;
            _genericRepository = genericRepository;
            }
        public async Task<User> UpdateModel(UpdateProfile updateProfile, string id)
        {

            var GetTheId = await _genericRepository.GetById(id);
            if (GetTheId == null)
            { 
                throw new Exception("Entity not found with the provided id.");
            }


            if (updateProfile.Image?.Length > 0)
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath + "\\uploads\\");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = updateProfile.Image.FileName;
                string filePath = Path.Combine(path, filename);

                using (FileStream filestream = new FileStream(filePath, FileMode.Create))
                {
                    await updateProfile.Image.CopyToAsync(filestream);
                    GetTheId.Image = "https://localhost:44357/uploads/" + filename;
                }

            }
            var result = await _genericRepository.Update(GetTheId);
            return result;
        }
    }
}
