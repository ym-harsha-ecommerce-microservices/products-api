using eCommerce.BLL.Mappers;
using eCommerce.BLL.Services.Contracts;
using eCommerce.BLL.Services.Implementations;
using eCommerce.BLL.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.BLL;

/// <summary>
/// Provides an extension method for registering the Business Logic Layer's
/// services, mapping profiles, and validators into the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Business Logic Layer dependencies, including <see cref="IProductService"/>,
    /// AutoMapper profiles, FluentValidation validators, and automatic validation for incoming requests.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, for chaining.</returns>
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        services.AddAutoMapper(confg =>
        {
            confg.AddMaps(typeof(ProductMappingProfile).Assembly);
        });

        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();

        services.AddFluentValidationAutoValidation();

        return services;
    }
}