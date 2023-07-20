using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.Models
{
    public class Products
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Name is Requires")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Price is Requires")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Description is Requires")]

        public string Description   { get; set; }
        [Required(ErrorMessage = "Category is Requires")]
        public string Category { get; set; }
        //[Required(ErrorMessage = "image")]
        public string Image { get; set; }
       
    }
}
