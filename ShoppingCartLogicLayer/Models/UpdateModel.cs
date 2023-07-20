using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.Models
{
    public class UpdateModel
    {
        [Required(ErrorMessage = "Name is Requires")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Price is Requires")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Description is Requires")]

        public string Description { get; set; }
        
        [Required(ErrorMessage = "Category is Requires")]
        public string Category { get; set; }
        //[Required(ErrorMessage = "image is Required")]
        public IFormFile? Image { get; set; }
    }
}
