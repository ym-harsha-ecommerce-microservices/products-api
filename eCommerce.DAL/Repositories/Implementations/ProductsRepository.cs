using eCommerce.DAL.DbContexts;
using eCommerce.DAL.Entities;
using eCommerce.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eCommerce.DAL.Repositories.Implementations;

internal class ProductsRepository(ApplicationDbContext context) : IProductsRepository
{
    private readonly ApplicationDbContext _context = context;

    /// <inheritdoc/>
    public async Task<Product?> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        var rowAffected = await _context.SaveChangesAsync();
        if (rowAffected > 0)
            return product;
        return null;
    }
    /// <inheritdoc/>
    public async Task<Product?> DeleteAsync(Guid productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return null;
        }
        _context.Products.Remove(product);
        var rowAffected = await _context.SaveChangesAsync();
        if (rowAffected > 0)
        {
            return product;
        }
        return null;
    }
    /// <inheritdoc/>
    public async Task<Product?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition)
    {

        var product = await _context.Products.FirstOrDefaultAsync(condition);

        return product;
    }
    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products.AsNoTracking().ToListAsync();
    }
    /// <inheritdoc/>

    public async Task<IEnumerable<Product>> GetAllProductsByConditionAsync(Expression<Func<Product, bool>> condition)
    {

        var products = await _context.Products.AsNoTracking().Where(condition).ToListAsync();

        return products;
    }
    /// <inheritdoc/>
    public async Task<Product?> UpdateAsync(Product product)
    {

        _context.Entry<Product>(product).State = EntityState.Modified;
        var rowAffectded = await _context.SaveChangesAsync();
        if (rowAffectded > 0)
            return product;
        return null;
    }
}
