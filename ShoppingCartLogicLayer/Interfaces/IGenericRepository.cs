using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //Task<PaginatedResult<T>> GetByPages(int pageNumber, int pageSize);

        Task<List<T>> GetAll();
        Task<T> GetById(Guid id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<bool> Delete(Guid id);
       
    }
}

