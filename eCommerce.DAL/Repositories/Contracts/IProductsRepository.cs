using eCommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.DAL.Repositories.Contracts;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<IEnumerable<Product>> GetAllProductsByConditionAsync(Func<Product, bool> condition);
    Task<Product?> GetProductByConditionAsync(Func<Product, bool> condition);
    Task<Product?> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid productId);
}
