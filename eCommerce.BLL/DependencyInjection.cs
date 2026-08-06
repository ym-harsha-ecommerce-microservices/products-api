using eCommerce.BLL.Mappers;
using eCommerce.BLL.RabbitMQ;
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
        services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();


        services.Configure<RabbitMQOptions>(options =>
        {
            options.RABBITMQ_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            options.RABBITMQ_PORT = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
            options.RABBITMQ_USERNAME = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest";
            options.RABBITMQ_PASSWORD = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";
            options.RABBITMQ_PRODUCT_EXCHANGE = Environment.GetEnvironmentVariable("RABBITMQ_PRODUCT_EXCHANGE") ?? "product.exchange";
            options.RABBITMQ_PRODUCT_UPDATE_NAME_ROUTEING_KEY = Environment.GetEnvironmentVariable("RABBITMQ_PRODUCT_UPDATE_NAME_ROUTEING_KEY") ?? "product.update.name";
            options.RABBITMQ_PRODUCT_DELETE_ROUTEING_KEY = Environment.GetEnvironmentVariable("RABBITMQ_PRODUCT_DELETE_ROUTEING_KEY") ?? "product.delete";
            options.RABBITMQ_PRODUCT_DELETE_QUEUE = Environment.GetEnvironmentVariable("RABBITMQ_PRODUCT_DELETE_QUEUE") ?? "product.delete.queue";
            options.RABBITMQ_PRODUCT_UPDATE_QUEUE = Environment.GetEnvironmentVariable("RABBITMQ_PRODUCT_UPDATE_QUEUE") ?? "product.update.queue";
        });

        services.AddAutoMapper(confg =>
        {
            confg.AddMaps(typeof(ProductMappingProfile).Assembly);
        });

        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();

        services.AddFluentValidationAutoValidation();

        return services;
    }
}