using System.Collections.Generic;
using System.Threading.Tasks;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests.Controllers;

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly Mock<ILogger<ProductController>> _logger;
    private readonly Mock<IProductService> _productService;

    public ProductControllerTests()
    {
        _productService = new Mock<IProductService>();
        _logger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_logger.Object, _productService.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsOk_WhenProductsExist()
    {
        // Arrange
        var expectedProducts = new List<ProductResponse>
        {
            new() { Name = "Sausage Roll", FormattedPrice = "£1.00", RawPrice = 1m },
            new() { Name = "Yum Yum", FormattedPrice = "£0.70", RawPrice = 0.7m }
        };

        _productService
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
        Assert.Equal(expectedProducts, products);
    }

    [Fact]
    public async Task GetProducts_ReturnsOk_WhenNoProductsExist()
    {
        // Arrange
        _productService
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ReturnsAsync(new List<ProductResponse>());

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(okResult.Value);
        Assert.Empty(products);
    }

    [Fact]
    public async Task GetProducts_CallsService_WithCorrectParameters()
    {
        // Arrange
        const int pageStart = 2;
        const int pageSize = 3;
        const Currency currency = Currency.Eur;

        _productService
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ReturnsAsync(new List<ProductResponse>());

        // Act
        await _controller.GetProducts(pageStart, pageSize, currency);

        // Assert
        _productService.Verify(x => x.GetProducts(pageStart, pageSize, currency), Times.Once);
    }

    [Fact]
    public async Task GetProducts_UsesDefaultParameters_WhenNotProvided()
    {
        // Arrange
        _productService
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ReturnsAsync(new List<ProductResponse>());

        // Act
        await _controller.GetProducts();

        // Assert
        _productService.Verify(x => x.GetProducts(0, 5, Currency.Gbp), Times.Once);
    }

    [Fact]
    public async Task GetProducts_Returns200StatusCode()
    {
        // Arrange
        _productService
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ReturnsAsync(new List<ProductResponse>());

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    }
}