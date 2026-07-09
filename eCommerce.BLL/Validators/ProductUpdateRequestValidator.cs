// Ignore Spelling: Validator

using eCommerce.BLL.DTOs;
using eCommerce.DAL.Repositories.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.BLL.Validators;

public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
{
    private readonly IProductsRepository productsRepository;

    public ProductUpdateRequestValidator([FromServices] IProductsRepository productsRepository)
    {
        RuleFor(p => p.ProductID)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(p => p.ProductName)
            .NotEmpty().WithMessage("Product Name is required.")
            .MaximumLength(100).WithMessage("Product Name must not exceed 100 characters.");

        RuleFor(p => p.Category)
            .NotEmpty().WithMessage("Category is required.")
            .IsInEnum().WithMessage("Category must be exist");

        RuleFor(p => p.UnitPrice)
            .NotNull().WithMessage("Unit Price is required.")
            .GreaterThan(0).WithMessage("Unit Price must be greater than zero.");

        RuleFor(p => p.QuantityInStock)
            .NotNull().WithMessage("Quantity in stock is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock cannot be negative.");
        this.productsRepository = productsRepository;
    }

}
