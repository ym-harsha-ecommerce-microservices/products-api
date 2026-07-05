using eCommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.DAL.Repositories.Contracts;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product?> GetProductByConditionAsync(Func<Product, bool> condition);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int productId);
}
