using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.Models
{
    public class PaginatedResult
    {
        public List<Products> Items { get; set; }
        public int TotalCount { get; set; }
    }
  
}
