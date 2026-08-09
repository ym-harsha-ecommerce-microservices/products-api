using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using eCommerce.BLL.DTOs;
using eCommerce.BLL.DTOs.RabbitMQMessages.ProductMessages;
using eCommerce.BLL.Exceptions;
using eCommerce.BLL.RabbitMQ;
using eCommerce.BLL.RabbitMQ.ProductMessages;
using eCommerce.BLL.Services.Implementations;
using eCommerce.DAL.Entities;
using eCommerce.DAL.Repositories.Contracts;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Timers;
using Xunit;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace eCommerce.Tests.ServicesTests;

public class ProductServiceTests
{
    private readonly IFixture _fixture;

    private readonly Mock<IProductsRepository> _productsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<ProductAddRequest>> _productAddValidatorMock;
    private readonly Mock<IValidator<ProductUpdateRequest>> _productUpdateValidatorMock;
    private readonly Mock<IRabbitMQPublisher> _rabbitMQPublisherMock;
    private readonly Mock<IOptions<RabbitMQOptions>> _rabbitMQOptionsMock;

    private readonly RabbitMQOptions _rabbitMQOptions;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _productsRepositoryMock = _fixture.Freeze<Mock<IProductsRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _productAddValidatorMock = _fixture.Freeze<Mock<IValidator<ProductAddRequest>>>();
        _productUpdateValidatorMock = _fixture.Freeze<Mock<IValidator<ProductUpdateRequest>>>();
        _rabbitMQPublisherMock = _fixture.Freeze<Mock<IRabbitMQPublisher>>();
        _rabbitMQOptionsMock = _fixture.Freeze<Mock<IOptions<RabbitMQOptions>>>();

        // By default, FluentValidation's mocked ValidateAsync returns a
        // ValidationResult with no failures, i.e. IsValid == true.
        // Tests that need a validation FAILURE override this per-test.
        _productAddValidatorMock
            .Setup(temp => temp.ValidateAsync(It.IsAny<ProductAddRequest>(), default))
            .ReturnsAsync(new ValidationResult());

        _productUpdateValidatorMock
            .Setup(temp => temp.ValidateAsync(It.IsAny<ProductUpdateRequest>(), default))
            .ReturnsAsync(new ValidationResult());

        // IOptions<T>.Value can't be auto-mocked usefully, so we build a real
        // RabbitMQOptions instance and wire it up manually.
        _rabbitMQOptions = _fixture.Create<RabbitMQOptions>();
        _rabbitMQOptionsMock.Setup(temp => temp.Value).Returns(_rabbitMQOptions);

        _productService = new ProductService(
            _productsRepositoryMock.Object,
            _mapperMock.Object,
            _productAddValidatorMock.Object,
            _productUpdateValidatorMock.Object,
            _rabbitMQPublisherMock.Object,
            _rabbitMQOptionsMock.Object);
    }

    #region CreateProductAsync

    [Fact]
    public async Task CreateProductAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        Func<Task> action = async () => await _productService.CreateProductAsync(null!);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateProductAsync_InvalidRequest_ThrowsCustomValidationException()
    {
        // Arrange
        ProductAddRequest productAddRequest = _fixture.Create<ProductAddRequest>();

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("ProductName", "Product name is required")
        };

        _productAddValidatorMock
            .Setup(temp => temp.ValidateAsync(productAddRequest, default))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        Func<Task> action = async () => await _productService.CreateProductAsync(productAddRequest);

        // Assert
        await action.Should().ThrowAsync<CustomValidationException>();
    }

    [Fact]
    public async Task CreateProductAsync_RepositoryReturnsNull_ToBeNull()
    {
        // Arrange
        ProductAddRequest productAddRequest = _fixture.Create<ProductAddRequest>();
        Product mappedProduct = _fixture.Create<Product>();

        _mapperMock
            .Setup(temp => temp.Map<Product>(productAddRequest))
            .Returns(mappedProduct);

        _productsRepositoryMock
            .Setup(temp => temp.CreateAsync(mappedProduct))
            .ReturnsAsync(null as Product);

        // Act
        ProductResponse? result = await _productService.CreateProductAsync(productAddRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateProductAsync_ValidRequest_ToBeSuccessful()
    {
        // Arrange
        ProductAddRequest productAddRequest = _fixture.Create<ProductAddRequest>();
        Product mappedProduct = _fixture.Create<Product>();
        ProductResponse productResponse = _fixture.Create<ProductResponse>();

        _mapperMock
            .Setup(temp => temp.Map<Product>(productAddRequest))
            .Returns(mappedProduct);

        _productsRepositoryMock
            .Setup(temp => temp.CreateAsync(mappedProduct))
            .ReturnsAsync(mappedProduct);

        _mapperMock
            .Setup(temp => temp.Map<ProductResponse>(mappedProduct))
            .Returns(productResponse);

        // Act
        ProductResponse? result = await _productService.CreateProductAsync(productAddRequest);

        // Assert
        result.Should().Be(productResponse);
    }

    #endregion

    #region DeleteProductAsync

    [Fact]
    public async Task DeleteProductAsync_ProductNotFound_ToBeFalse()
    {
        // Arrange
        Guid productId = _fixture.Create<Guid>();

        _productsRepositoryMock
            .Setup(temp => temp.DeleteAsync(productId))
            .ReturnsAsync(null as Product);

        // Act
        bool result = await _productService.DeleteProductAsync(productId);

        // Assert
        result.Should().BeFalse();

        // A deletion that never happened should never publish a message.
        _rabbitMQPublisherMock.Verify(
            temp => temp.PublishAsync(It.IsAny<ProductDeleteMessage>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteProductAsync_ProductFound_ToBeTrue_AndPublishesMessage()
    {
        // Arrange
        Guid productId = _fixture.Create<Guid>();
        Product product = _fixture.Build<Product>()
            .With(temp => temp.ProductID, productId)
            .Create();

        _productsRepositoryMock
            .Setup(temp => temp.DeleteAsync(productId))
            .ReturnsAsync(product);

        // Act
        bool result = await _productService.DeleteProductAsync(productId);

        // Assert
        result.Should().BeTrue();

        _rabbitMQPublisherMock.Verify(
            temp => temp.PublishAsync(
                It.Is<ProductDeleteMessage>(msg =>
                    msg.ProductId == productId && msg.ProductName == product.ProductName),
                _rabbitMQOptions.RABBITMQ_PRODUCT_DELETE_ROUTEING_KEY),
            Times.Once);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ToBeSuccessful()
    {
        // Arrange
        List<Product> products = _fixture.CreateMany<Product>(3).ToList();
        List<ProductResponse> productResponses = _fixture.CreateMany<ProductResponse>(3).ToList();

        _productsRepositoryMock
            .Setup(temp => temp.GetProductsAsync())
            .ReturnsAsync(products);

        _mapperMock
            .Setup(temp => temp.Map<IEnumerable<ProductResponse>>(products))
            .Returns(productResponses);

        // Act
        IEnumerable<ProductResponse> result = await _productService.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(productResponses);
    }

    #endregion

    #region GetAllProductsByConditionAsync

    [Fact]
    public async Task GetAllProductsByConditionAsync_ToBeSuccessful()
    {
        // Arrange
        Expression<Func<Product, bool>> condition = p => p.Category == "Electronics";

        List<Product> products = _fixture.CreateMany<Product>(2).ToList();
        List<ProductResponse> productResponses = _fixture.CreateMany<ProductResponse>(2).ToList();

        _productsRepositoryMock
            .Setup(temp => temp.GetAllProductsByConditionAsync(condition))
            .ReturnsAsync(products);

        _mapperMock
            .Setup(temp => temp.Map<IEnumerable<ProductResponse>>(products))
            .Returns(productResponses);

        // Act
        IEnumerable<ProductResponse> result = await _productService.GetAllProductsByConditionAsync(condition);

        // Assert
        result.Should().BeEquivalentTo(productResponses);
    }

    #endregion

    #region GetProductByConditionAsync

    [Fact]
    public async Task GetProductByConditionAsync_ProductNotFound_ToBeNull()
    {
        // Arrange
        Expression<Func<Product, bool>> condition = p => p.ProductID == Guid.NewGuid();

        _productsRepositoryMock
            .Setup(temp => temp.GetProductByConditionAsync(condition))
            .ReturnsAsync(null as Product);

        // Act
        ProductResponse? result = await _productService.GetProductByConditionAsync(condition);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProductByConditionAsync_ProductFound_ToBeSuccessful()
    {
        // Arrange
        Expression<Func<Product, bool>> condition = p => p.ProductID == Guid.NewGuid();
        Product product = _fixture.Create<Product>();
        ProductResponse productResponse = _fixture.Create<ProductResponse>();

        _productsRepositoryMock
            .Setup(temp => temp.GetProductByConditionAsync(condition))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(temp => temp.Map<ProductResponse>(product))
            .Returns(productResponse);

        // Act
        ProductResponse? result = await _productService.GetProductByConditionAsync(condition);

        // Assert
        result.Should().Be(productResponse);
    }

    #endregion

    #region UpdateProductAsync

    [Fact]
    public async Task UpdateProductAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        Func<Task> action = async () => await _productService.UpdateProductAsync(null!);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateProductAsync_InvalidRequest_ThrowsCustomValidationException()
    {
        // Arrange
        ProductUpdateRequest productUpdateRequest = _fixture.Create<ProductUpdateRequest>();

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("ProductName", "Product name is required")
        };

        _productUpdateValidatorMock
            .Setup(temp => temp.ValidateAsync(productUpdateRequest, default))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        Func<Task> action = async () => await _productService.UpdateProductAsync(productUpdateRequest);

        // Assert
        await action.Should().ThrowAsync<CustomValidationException>();
    }

    [Fact]
    public async Task UpdateProductAsync_ProductNotFound_ThrowsCustomValidationException()
    {
        // Arrange
        ProductUpdateRequest productUpdateRequest = _fixture.Create<ProductUpdateRequest>();

        _productsRepositoryMock
            .Setup(temp => temp.GetProductByConditionAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(null as Product);

        // Act
        Func<Task> action = async () => await _productService.UpdateProductAsync(productUpdateRequest);

        // Assert
        await action.Should().ThrowAsync<CustomValidationException>();
    }

    [Fact]
    public async Task UpdateProductAsync_NameUnchanged_DoesNotPublishMessage()
    {
        // Arrange
        Product existingProduct = _fixture.Create<Product>();

        // Deliberately give the update request the SAME name as the
        // existing product, so the service's "isProductNameChanged" check
        // evaluates to false and no RabbitMQ message should be published.
        ProductUpdateRequest productUpdateRequest = _fixture.Build<ProductUpdateRequest>()
            .With(temp => temp.ProductID, existingProduct.ProductID)
            .With(temp => temp.ProductName, existingProduct.ProductName)
            .Create();

        ProductResponse productResponse = _fixture.Create<ProductResponse>();

        _productsRepositoryMock
            .Setup(temp => temp.GetProductByConditionAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(existingProduct);

        _productsRepositoryMock
            .Setup(temp => temp.UpdateAsync(existingProduct))
            .ReturnsAsync(existingProduct);

        _mapperMock
            .Setup(temp => temp.Map<ProductResponse>(existingProduct))
            .Returns(productResponse);

        // Act
        ProductResponse? result = await _productService.UpdateProductAsync(productUpdateRequest);

        // Assert
        result.Should().Be(productResponse);

        _rabbitMQPublisherMock.Verify(
            temp => temp.PublishAsync(It.IsAny<ProductNameUpdateMessage>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateProductAsync_NameChanged_PublishesMessage()
    {
        // Arrange
        Product existingProduct = _fixture.Build<Product>()
            .With(temp => temp.ProductName, "Old Name")
            .Create();

        ProductUpdateRequest productUpdateRequest = _fixture.Build<ProductUpdateRequest>()
            .With(temp => temp.ProductID, existingProduct.ProductID)
            .With(temp => temp.ProductName, "New Name")
            .Create();

        ProductResponse productResponse = _fixture.Create<ProductResponse>();

        _productsRepositoryMock
            .Setup(temp => temp.GetProductByConditionAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(existingProduct);

        _productsRepositoryMock
            .Setup(temp => temp.UpdateAsync(existingProduct))
            .ReturnsAsync(existingProduct);

        _mapperMock
            .Setup(temp => temp.Map<ProductResponse>(existingProduct))
            .Returns(productResponse);

        // Act
        ProductResponse? result = await _productService.UpdateProductAsync(productUpdateRequest);

        // Assert
        result.Should().Be(productResponse);

        _rabbitMQPublisherMock.Verify(
            temp => temp.PublishAsync(
                It.Is<ProductNameUpdateMessage>(msg => msg.ProductId == existingProduct.ProductID),
                _rabbitMQOptions.RABBITMQ_PRODUCT_UPDATE_NAME_ROUTEING_KEY),
            Times.Once);
    }

    #endregion
}