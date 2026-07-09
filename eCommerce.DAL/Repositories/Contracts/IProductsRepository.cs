using eCommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace eCommerce.DAL.Repositories.Contracts;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<IEnumerable<Product>> GetAllProductsByConditionAsync(Expression<Func<Product, bool>> condition);
    Task<Product?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition);
    Task<Product?> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid productId);
}
