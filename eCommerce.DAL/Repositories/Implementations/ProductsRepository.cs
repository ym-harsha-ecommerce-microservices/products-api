using eCommerce.DAL.DbContexts;
using eCommerce.DAL.Entities;
using eCommerce.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.DAL.Repositories.Implementations;

internal class ProductsRepository(ApplicationDbContext context) : IProductsRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return;
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> GetProductByConditionAsync(Func<Product, bool> condition)
    {

        var product = await _context.Products.FirstOrDefaultAsync(product => condition.Invoke(product));

        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllProductsByConditionAsync(Func<Product, bool> condition)
    {
        var products = await _context.Products.AsNoTracking().Where(product => condition(product)).ToListAsync();

        return products;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Entry<Product>(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
