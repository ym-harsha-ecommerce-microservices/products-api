using AutoMapper;
using eCommerce.BLL.DTOs;
using eCommerce.BLL.Services.Contracts;
using eCommerce.DAL.Entities;
using eCommerce.DAL.Repositories.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Services.Implementations;

public class ProductService(IProductsRepository productsRepository, IMapper mapper, IValidator<ProductAddRequest> productAddValidator, IValidator<ProductUpdateRequest> productUpdateValidator) : IProductService
{
    public async Task<ProductResponse?> CreateProductAsync(ProductAddRequest productAddRequest)
    {
        if (productAddRequest == null)
            throw new ArgumentNullException(nameof(productAddRequest));

        var result = await productAddValidator.ValidateAsync(productAddRequest);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentException(errors);
        }

        var product = mapper.Map<Product>(productAddRequest);
        var addedProduct = await productsRepository.CreateAsync(product);

        if (addedProduct == null)
            return null;

        return mapper.Map<ProductResponse>(product);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        return await productsRepository.DeleteAsync(id);
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
    public async Task<ProductResponse?> GetProductByConditionAsync(Func<Product, bool> condition)
    {
        var product = await productsRepository.GetProductByConditionAsync(condition);

        if (product == null)
            return null;

        return mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest productUpdateRequest)
    {
        if (productUpdateRequest == null)
            throw new ArgumentNullException(nameof(productUpdateRequest));

        var result = await productUpdateValidator.ValidateAsync(productUpdateRequest);
        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }


        var product = await productsRepository.GetProductByConditionAsync(p => p.ProductID == productUpdateRequest.ProductID);

        if (product == null) throw new ArgumentException("Invalid Product ID");

        mapper.Map(productUpdateRequest, product);

        await productsRepository.UpdateAsync(product);

        return mapper.Map<ProductResponse>(product);
    }
}
