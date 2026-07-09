using eCommerce.BLL.DTOs;
using eCommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Services.Contracts;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<IEnumerable<ProductResponse>> GetAllProductsByConditionAsync(Expression<Func<Product, bool>> condition);
    Task<ProductResponse?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition);
    Task<ProductResponse?> CreateProductAsync(ProductAddRequest productAddRequest);
    Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest productUpdateRequest);
    Task<bool> DeleteProductAsync(Guid id);

}
