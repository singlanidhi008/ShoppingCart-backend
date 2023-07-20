using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartBusinessLayer.Services
{
    public class UserServices
    {
        readonly IGenericRepository<Products> genericRepository;
        readonly IWebHostEnvironment _webHostEnvironment;

        public UserServices( IGenericRepository<Products> genericRepository,IWebHostEnvironment webHostEnvironment)
        {
            this.genericRepository = genericRepository;
            this._webHostEnvironment = webHostEnvironment;
        }

        public Task<List<Products>> GetModel()
        {
            var result=genericRepository.GetAll();
            return result;
        }


        public async Task<Products> AddModel(AddModel addModel)
        {

            var data = new Products()
            {
                Id = Guid.NewGuid(),
                Name = addModel.Name,
                Price = addModel.Price,
                Category = addModel.Category,
                Description = addModel.Description
            };

            if (addModel.Image.Length > 0)
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath + "\\uploads\\");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = addModel.Image.FileName;
                string filePath = Path.Combine(path, filename);


                using (FileStream filestream = new FileStream(filePath, FileMode.Create))
                {
                    await addModel.Image.CopyToAsync(filestream);
                    data.Image = "https://localhost:44357/uploads/" + filename;
                }
            }


            var addedContact = await genericRepository.Add(data);
            return addedContact;

        }


        public async Task<Products> UpdateModel(UpdateModel updateModel, Guid id)
        {
            var GetTheId = await genericRepository.GetById(id);
            if ( GetTheId != null)
            {
                GetTheId.Name = updateModel.Name;
                GetTheId.Price = updateModel.Price;
                GetTheId.Category = updateModel.Category;
                GetTheId.Description = updateModel.Description;

            }

            if (updateModel.Image?.Length > 0)
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath + "\\uploads\\");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = updateModel.Image.FileName;
                string filePath = Path.Combine(path, filename);

                using (FileStream filestream = new FileStream(filePath, FileMode.Create))
                {
                    await updateModel.Image.CopyToAsync(filestream);
                    GetTheId.Image = "https://localhost:44357/uploads/" + filename;
                }

            }
            var result = await genericRepository.Update(GetTheId);
            return result;

        }

        public Task<Products> GetId(Guid id)
        {
            var result =  genericRepository.GetById(id);
            return result;
        }

       
    }
}
