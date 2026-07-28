using eCommerce.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.DAL.DbContexts;

/// <summary>
/// Represents the Entity Framework Core database context for the application,
/// providing access to the <see cref="Products"/> table.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationDbContext"/> with the specified options.
    /// </summary>
    /// <param name="options">The options used to configure this context.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    /// <summary>
    /// Gets or sets the <see cref="Product"/> entities.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

}