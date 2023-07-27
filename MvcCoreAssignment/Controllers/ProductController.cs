using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartBusinessLayer.Services;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using Microsoft.AspNetCore.Http;

namespace MvcCoreAssignment.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
  
    public class ProductController : ControllerBase
    {
        private const string Error = "Id does not Exist";
        public static IWebHostEnvironment _webHostEnvironment;
        private readonly IGenericRepository<Products> genericRepository;
        private readonly ProductServices productServices;
        private readonly UserServices _userServices;
        
       
        public ProductController(IGenericRepository<Products> genericRepository,ProductServices productServices,IWebHostEnvironment webHostEnvironment,UserServices userServices)
        {
            this.genericRepository = genericRepository;
            this.productServices= productServices;
            _webHostEnvironment = webHostEnvironment;
            this._userServices = userServices;
           
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userServices.GetModel();
            if(result==null)
            {
                return BadRequest("No Product is there");
            }
            return Ok(result);
        }
        [HttpGet("GetAllProducts")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<List<Products>>> GetAllProducts(string? searchString = "", int pageSize = 3, int pageNumber = 1, string orderBy = "name")
        {
            var result = await productServices.Index(searchString, pageSize, pageNumber, orderBy);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] AddModel addModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request. Please provide valid data.");
            }

            var result = await _userServices.AddModel(addModel);

            return Ok(result);

        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromForm] UpdateModel updateModel, [FromRoute] Guid id)
        {
            var result = await _userServices.UpdateModel(updateModel, id);
              if(result==null)
            {
                return BadRequest("Product not updated");
            }
            return Ok(result);
            
        }
         [HttpGet("GetById/{id:guid}")]
        //[Route("{id:guid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _userServices.GetId(id);
            if(result==null)
            {
                return BadRequest("iD does not Exist");
            }
            return Ok(result);

        }

        [HttpDelete("DeleteProduct/{id}")]
        //[Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var GetTheId = await _userServices.GetId(id);
            if(GetTheId!=null)
            {
                await genericRepository.Delete(id);
                return Ok();
            }
            return BadRequest("id does not exist");
        }
        [HttpGet("ExportProductsToExcel")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
                
                var excelContent = await _userServices.ExportProductsToExcel();
                var fileName = "products.xlsx";
                //var contentDisposition = new System.Net.Mime.ContentDisposition
                //{
                //    FileName = fileName,
                //    Inline = false
                //};
                //Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
                return File(excelContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            
        }
        [HttpPost("ImportData")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> ImportData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected for import.");
            }

            var result = await _userServices.ImportDataFromFile(file);
            return Ok(new { message = result });
        }



    }
}

