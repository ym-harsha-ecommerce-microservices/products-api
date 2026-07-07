using eCommerce.BLL.DTOs;
using eCommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Services.Contracts;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<IEnumerable<ProductResponse>> GetAllProductsByConditionAsync(Func<Product, bool> condition);
    Task<ProductResponse> AddProductAsync(ProductAddRequest productAddRequest);
    Task<ProductResponse> UpdateProductAsync(ProductUpdateRequest productUpdateRequest);
    Task DeleteProductAsync(Guid id);

}
