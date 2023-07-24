using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartDataLayer.DbContext;
using ShoppingCartDataLayer.Interfaces;
using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ShoppingCartBusinessLayer.Services
{
  
        public class GenericRepository<T> : IGenericRepository<T> where T : class
        {
            private readonly ShoppingCartDbContext dbContext;
            private readonly DbSet<T> dbSet;

             public GenericRepository(ShoppingCartDbContext dbContext)
             {
                this.dbContext = dbContext;
                this.dbSet = dbContext.Set<T>();
             }
             public async Task<T> Add(T entity)
             {
                await dbSet.AddAsync(entity);
                await dbContext.SaveChangesAsync();
                return entity;
             }

              public async Task<bool> Delete(Guid id)
              {
                var entity = await dbSet.FindAsync(id);
                if (entity != null)
                {
                    dbSet.Remove(entity);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
              }

               public async Task<T> GetById(object id)
                {
               var res = await dbSet.FindAsync(id);

                return res;
                 }

                public async Task<List<T>> GetAll()
                {
                     return await dbSet.ToListAsync();
                }

        public async Task<T> Update(T entity)
            {
            dbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return entity;
        }

    }
    }

