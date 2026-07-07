// Ignore Spelling: Validator

using eCommerce.BLL.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.Validators;

public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
{
    public ProductAddRequestValidator()
    {

        RuleFor(p => p.ProductName)
            .NotEmpty().WithMessage("Product Name is required.")
            .MaximumLength(100).WithMessage("Product Name must not exceed 100 characters.");

        RuleFor(p => p.Category)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(p => p.UnitPrice)
            .NotNull().WithMessage("Unit Price is required.")
            .GreaterThan(0).WithMessage("Unit Price must be greater than zero.");

        RuleFor(p => p.QuantityInStock)
            .NotNull().WithMessage("Quantity in stock is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock cannot be negative.");
    }
}
