using AutoMapper;
using eCommerce.BLL.DTOs;
using eCommerce.BLL.DTOs.RabbitMQMessages.ProductMessages;
using eCommerce.BLL.Exceptions;
using eCommerce.BLL.RabbitMQ;
using eCommerce.BLL.RabbitMQ.ProductMessages;
using eCommerce.BLL.Services.Contracts;
using eCommerce.DAL.Entities;
using eCommerce.DAL.Repositories.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Services.Implementations;

public class ProductService(IProductsRepository _productsRepository,
    IMapper _mapper,
    IValidator<ProductAddRequest> _productAddValidator,
    IValidator<ProductUpdateRequest> _productUpdateValidator,
    IRabbitMQPublisher _rabbitMQPublisher,
    IOptions<RabbitMQOptions> _rabbitMQOptions) : IProductService
{

    /// <inheritdoc/>
    public async Task<ProductResponse?> CreateProductAsync(ProductAddRequest productAddRequest)
    {
        if (productAddRequest == null)
            throw new ArgumentNullException(nameof(productAddRequest));

        var result = await _productAddValidator.ValidateAsync(productAddRequest);

        if (!result.IsValid)
        {

            var errors = result.Errors.GroupBy(temp => temp.PropertyName)
                .ToDictionary(grp => grp.Key, grp => grp.Select(err => err.ErrorMessage).ToArray());

            throw new CustomValidationException(errors);
        }

        var product = _mapper.Map<Product>(productAddRequest);
        var addedProduct = await _productsRepository.CreateAsync(product);

        if (addedProduct == null)
            return null;

        return _mapper.Map<ProductResponse>(product);
    }
    /// <inheritdoc/>
    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _productsRepository.DeleteAsync(id);

        if (product != null)
        {
            var message = new ProductDeleteMessage
            {
                ProductId = id,
                ProductName = product.ProductName!
            };
            string routingKey = _rabbitMQOptions.Value.RABBITMQ_PRODUCT_DELETE_ROUTEING_KEY;

            await _rabbitMQPublisher.PublishAsync(message, routingKey);

            return true;
        }
        return false;

    }
    /// <inheritdoc/>
    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productsRepository.GetProductsAsync();
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }
    /// <inheritdoc/>
    public async Task<IEnumerable<ProductResponse>> GetAllProductsByConditionAsync(Expression<Func<Product, bool>> condition)
    {
        var products = await _productsRepository.GetAllProductsByConditionAsync(condition);
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }
    /// <inheritdoc/>
    public async Task<ProductResponse?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition)
    {
        var product = await _productsRepository.GetProductByConditionAsync(condition);

        if (product == null)
            return null;

        return _mapper.Map<ProductResponse>(product);
    }
    /// <inheritdoc/>
    public async Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest productUpdateRequest)
    {
        if (productUpdateRequest == null)
            throw new ArgumentNullException(nameof(productUpdateRequest));

        var result = await _productUpdateValidator.ValidateAsync(productUpdateRequest);
        if (!result.IsValid)
        {

            var errors = result.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(grp => grp.Key, grp => grp.Select(err => err.ErrorMessage).ToArray());

            throw new CustomValidationException(errors);
        }


        var product = await _productsRepository.GetProductByConditionAsync(p => p.ProductID == productUpdateRequest.ProductID);

        if (product == null) throw new CustomValidationException("Global", "Invalid Product ID");

        bool isProductNameChanged = productUpdateRequest.ProductName != product.ProductName;

        _mapper.Map(productUpdateRequest, product);

        await _productsRepository.UpdateAsync(product);


        if (isProductNameChanged)
        {
            string routingKey = _rabbitMQOptions.Value.RABBITMQ_PRODUCT_UPDATE_NAME_ROUTEING_KEY;
            var message = new ProductNameUpdateMessage
            {
                ProductId = product.ProductID,
                ProductNewName = product.ProductName!
            };
            await _rabbitMQPublisher.PublishAsync(message, routingKey);
        }

        return _mapper.Map<ProductResponse>(product);
    }
}
