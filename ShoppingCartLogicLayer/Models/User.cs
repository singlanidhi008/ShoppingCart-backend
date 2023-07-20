using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.Models
{
    public class User:IdentityUser
    {
        [Required(ErrorMessage ="Enter First Name Please")]
        public string FirstName { get; set; }=string.Empty;
        [Required(ErrorMessage ="Enter Last Name Please")]
        public string LastName { get; set;} = string.Empty;
    }
}
