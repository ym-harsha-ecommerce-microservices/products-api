using eCommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace eCommerce.DAL.Repositories.Contracts;

/// <summary>
/// Represents a contract for data access operations related to products.
/// </summary>
public interface IProductsRepository
{
    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A <see cref="Task"/> containing all <see cref="Product"/> entities.</returns>
    Task<IEnumerable<Product>> GetProductsAsync();

    /// <summary>
    /// Retrieves all products matching the specified condition.
    /// </summary>
    /// <param name="condition">An expression used to filter products.</param>
    /// <returns>A <see cref="Task"/> containing the matching <see cref="Product"/> entities.</returns>
    Task<IEnumerable<Product>> GetAllProductsByConditionAsync(Expression<Func<Product, bool>> condition);

    /// <summary>
    /// Retrieves a single product matching the specified condition.
    /// </summary>
    /// <param name="condition">An expression used to locate the product.</param>
    /// <returns>A <see cref="Task"/> containing the matching <see cref="Product"/> if found; otherwise, <c>null</c>.</returns>
    Task<Product?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="product">The product entity to create.</param>
    /// <returns>A <see cref="Task"/> containing the created <see cref="Product"/> if successful; otherwise, <c>null</c>.</returns>
    Task<Product?> CreateAsync(Product product);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product entity with updated values.</param>
    /// <returns>A <see cref="Task"/> containing the updated <see cref="Product"/> if successful; otherwise, <c>null</c>.</returns>
    Task<Product?> UpdateAsync(Product product);

    /// <summary>
    /// Deletes a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to delete.</param>
    /// <returns>A <see cref="Task"/> containing <c>true</c> if the product was deleted; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteAsync(Guid productId);
}