using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.Interfaces
{
    public interface IAccountRepository
    {
        Task<LoginREsModel> Login(LoginModelDto model);
    }
}
