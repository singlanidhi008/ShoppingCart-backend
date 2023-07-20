using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.Interfaces
{
   public interface ITokenService
    {
        string BuildToken(string key, string issuer, LoginModelDto user);
    }
}
