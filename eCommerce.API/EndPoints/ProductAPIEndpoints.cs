using eCommerce.BLL.DTOs;
using eCommerce.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.EndPoints;

/// <summary>
/// Defines minimal API endpoints for managing products.
/// </summary>
public static class ProductAPIEndpoints
{
    /// <summary>
    /// Maps all product-related endpoints (get, search, create, update, delete)
    /// onto the "/api/products" route group.
    /// </summary>
    /// <param name="app">The endpoint route builder to map endpoints onto.</param>
    public static void MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapGet("/", async (IProductService productService) =>
        {
            var products = await productService.GetAllAsync();
            return Results.Ok(products);
        })
        .WithSummary("Get all products")
        .WithDescription("Retrieves all products.");

        group.MapGet("/search/product-id/{productId:guid}", async ([FromServices] IProductService productService,
            [FromRoute] Guid productId) =>
        {
            var product = await productService.GetProductByConditionAsync(p => p.ProductID == productId);

            if (product == null)
                return Results.NotFound();

            return Results.Ok(product);
        })
        .WithSummary("Get product by ID")
        .WithDescription("Retrieves a specific product by its unique identifier.");

        group.MapGet("/products/search/{searchString}", async ([FromServices] IProductService productService,
            [FromRoute] string searchString) =>
        {
            var products = await productService.GetAllProductsByConditionAsync(p =>
                p.Category!.Contains(searchString) ||
                p.ProductName!.Contains(searchString));

            return Results.Ok(products);

        })
        .WithSummary("Search products")
        .WithDescription("Retrieves products whose category or name contains the given search string.");

        group.MapPost("/", async ([FromServices] IProductService productService,
            [FromBody] ProductAddRequest productAddRequest) =>
        {
            var productResponse = await productService.CreateProductAsync(productAddRequest);

            if (productResponse == null)
                return Results.BadRequest("Invalid product data.");

            return Results.Created($"/api/products/search/product-id/{productResponse.ProductID}", productResponse);
        })
        .WithSummary("Create a product")
        .WithDescription("Creates a new product with the given details.");

        group.MapPut("/", async ([FromServices] IProductService productService,
            [FromBody] ProductUpdateRequest productUpdateRequest) =>
        {
            var productResponse = await productService.UpdateProductAsync(productUpdateRequest);
            if (productResponse == null)
                return Results.NotFound("Product not found to update.");
            return Results.Ok(productResponse);
        })
        .WithSummary("Update a product")
        .WithDescription("Updates an existing product with the given details.");

        group.MapDelete("/{productId:guid}", async ([FromServices] IProductService productService,
            [FromRoute] Guid productId) =>
        {
            var isDeleted = await productService.DeleteProductAsync(productId);

            if (isDeleted)
                return Results.Ok();

            return Results.BadRequest("Product not found or already deleted.");
        })
        .WithSummary("Delete a product")
        .WithDescription("Deletes a product by its unique identifier.");
    }
}