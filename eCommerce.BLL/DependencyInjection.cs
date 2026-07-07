using eCommerce.BLL.Mappers;
using eCommerce.BLL.Services.Contracts;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddScoped<IProductService, IProductService>();

        services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);

        services.AddFluentValidationAutoValidation();

        return services;
    }
}
