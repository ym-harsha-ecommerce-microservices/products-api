using AutoMapper;
using eCommerce.BLL.DTOs;
using eCommerce.BLL.Services.Contracts;
using eCommerce.DAL.Entities;
using eCommerce.DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Services.Implementations;

public class ProductService(IProductsRepository productsRepository, IMapper mapper) : IProductService
{
    public async Task<ProductResponse> AddProductAsync(ProductAddRequest productAddRequest)
    {
        var product = mapper.Map<Product>(productAddRequest);
        await productsRepository.CreateAsync(product);
        return mapper.Map<ProductResponse>(product);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await productsRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await productsRepository.GetProductsAsync();
        return mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllProductsByConditionAsync(Func<Product, bool> condition)
    {
        var products = await productsRepository.GetAllProductsByConditionAsync(condition);
        return mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    public async Task<ProductResponse> UpdateProductAsync(ProductUpdateRequest productUpdateRequest)
    {
        if (productUpdateRequest?.ProductID == null) return null;

        var product = await productsRepository.GetProductByConditionAsync(p => p.ProductID == productUpdateRequest.ProductID);

        if (product == null) return null;

        mapper.Map(productUpdateRequest, product);

        await productsRepository.UpdateAsync(product);

        return mapper.Map<ProductResponse>(product);
    }
}
