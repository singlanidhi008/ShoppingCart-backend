using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartBusinessLayer.Services
{
    public class ProductServices
    {
        readonly IGenericRepository<Products> genericRepository;
        public ProductServices(IGenericRepository<Products> genericRepository)
        {
           this.genericRepository=genericRepository;
        }

        public async Task<PaginatedResult>Index(string? searchString = "", int pageSize = 3, int pageNumber = 1, string orderBy = "name")
        {
                string search = searchString != null ? searchString.ToLower() : "";
        var data = await genericRepository.GetAll();
        var searchedValue = data.Where(item => item.Name.ToLower().StartsWith(search) ||
                                     item.Price.ToString().StartsWith(search) ||
                                       item.Category.ToLower().StartsWith(search) ||
                                       item.Description.ToLower().StartsWith(search));
            switch(orderBy)
            {
                case "name_desc":
                    searchedValue= searchedValue.OrderByDescending(item => item.Name);
                    break;
                case "price_desc":
                    searchedValue = searchedValue.OrderByDescending(item => item.Price);
                    break;
                case "price_asc":
                    searchedValue = searchedValue.OrderBy(item => item.Price);
                    break;
                case "category_desc":
                    searchedValue = searchedValue.OrderByDescending(item => item.Category);
                    break;
                case "category_asc":
                    searchedValue = searchedValue.OrderBy(item => item.Category);
                    break;
                case "description_desc":
                    searchedValue = searchedValue.OrderByDescending(item => item.Description);
                    break;
                case "description_asc":
                    searchedValue = searchedValue.OrderBy(item => item.Description);
                    break;
                default:
                    searchedValue = searchedValue.OrderBy(item => item.Name);
                    break;

                   
            }
    var displayData = searchedValue.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    var countItem = new PaginatedResult()
    {
        Items = displayData,
        TotalCount = searchedValue.Count()
    };
            return countItem;


}
}
}
