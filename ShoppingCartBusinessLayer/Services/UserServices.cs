using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using ShoppingCartDataLayer.DbContext;
using System.Data.Entity;

namespace ShoppingCartBusinessLayer.Services
{
    public class UserServices
    {
        readonly IGenericRepository<Products> genericRepository;
        readonly IWebHostEnvironment _webHostEnvironment;
         readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ShoppingCartDbContext _dbContext;

        public UserServices( IGenericRepository<Products> genericRepository,IWebHostEnvironment webHostEnvironment,IHttpContextAccessor _httpContextAccessor, ShoppingCartDbContext dbContext)
        {
            this.genericRepository = genericRepository;
            this._webHostEnvironment = webHostEnvironment;
            this._dbContext = dbContext;
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

        public async Task<byte[]> ExportToExcel(List<Products> products)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");
                //worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Price";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Category";
                worksheet.Cells[1, 5].Value = "Image";

                int row = 2;
                foreach (var product in products)
                {
                    //worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 1].Value = product.Name;
                    worksheet.Cells[row, 2].Value = product.Price;
                    worksheet.Cells[row, 3].Value = product.Description;
                    worksheet.Cells[row, 4].Value = product.Category;
                    worksheet.Cells[row, 5].Value = product.Image;
                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> ExportProductsToExcel()
        {
            List<Products> data = await genericRepository.GetAll();
            return await ExportToExcel(data);
        }
        public async Task<string> ImportDataFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file selected for import.";
            }

            using (var stream = file.OpenReadStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                    {
                        return "Invalid Excel file format or empty worksheet.";
                    }
                    int rowCount = worksheet.Dimension.Rows;

                    List<Products> importedData = new List<Products>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string name = worksheet.Cells[row, 1].Value.ToString();
                        string priceValue = worksheet.Cells[row, 2].Value.ToString();
                        string description = worksheet.Cells[row, 3].Value.ToString();
                        string category = worksheet.Cells[row, 4].Value.ToString();
                        string image = worksheet.Cells[row, 5].Value.ToString();

                            Products product = new Products
                            {
                                Id = Guid.NewGuid(),
                                Name = name,
                                Price = 545,
                                Description = description,
                                Category = category,
                                Image = image
                            };

                            importedData.Add(product);
                       
                    }

                    foreach (var product in importedData)
                    {
                        
                        await genericRepository.Add(product);
                       
                    }
                    return $"Successfully imported {importedData.Count} products from the file.";
                }
            }
        }



        //public async Task SaveExcelToFile(string filePath)
        //{
        //    var excelContent = await ExportProductsToExcel();
        //    File.WriteAllBytes(filePath, excelContent);
        //}
    }
}
