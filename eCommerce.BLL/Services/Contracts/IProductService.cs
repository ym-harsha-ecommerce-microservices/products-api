using eCommerce.BLL.DTOs;
using eCommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Services.Contracts;

/// <summary>
/// Represents a contract for business logic operations related to products.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A <see cref="Task"/> containing all products as <see cref="ProductResponse"/> instances.</returns>
    Task<IEnumerable<ProductResponse>> GetAllAsync();

    /// <summary>
    /// Retrieves all products matching the specified condition.
    /// </summary>
    /// <param name="condition">An expression used to filter products.</param>
    /// <returns>A <see cref="Task"/> containing the matching products as <see cref="ProductResponse"/> instances.</returns>
    Task<IEnumerable<ProductResponse>> GetAllProductsByConditionAsync(Expression<Func<Product, bool>> condition);

    /// <summary>
    /// Retrieves a single product matching the specified condition.
    /// </summary>
    /// <param name="condition">An expression used to locate the product.</param>
    /// <returns>A <see cref="Task"/> containing the matching <see cref="ProductResponse"/> if found; otherwise, <c>null</c>.</returns>
    Task<ProductResponse?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="productAddRequest">The details of the product to create.</param>
    /// <returns>A <see cref="Task"/> containing the created <see cref="ProductResponse"/> if successful; otherwise, <c>null</c>.</returns>
    Task<ProductResponse?> CreateProductAsync(ProductAddRequest productAddRequest);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="productUpdateRequest">The updated details of the product.</param>
    /// <returns>A <see cref="Task"/> containing the updated <see cref="ProductResponse"/> if successful; otherwise, <c>null</c>.</returns>
    Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest productUpdateRequest);

    /// <summary>
    /// Deletes a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>A <see cref="Task"/> containing <c>true</c> if the product was deleted; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteProductAsync(Guid id);

}