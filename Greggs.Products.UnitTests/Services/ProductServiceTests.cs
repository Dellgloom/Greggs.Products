using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Repositories.Interfaces;
using Greggs.Products.Api.Services;
using Greggs.Products.Api.Services.Interfaces;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests.Services;

public class ProductServiceTests
{
    private static readonly List<Product> Products = new()
    {
        new Product { Name = "Sausage Roll", PriceInPounds = 1m },
        new Product { Name = "Vegan Sausage Roll", PriceInPounds = 1.1m },
        new Product { Name = "Steak Bake", PriceInPounds = 1.2m }
    };

    private readonly Mock<ICurrencyExchangeService> _currencyExchangeService;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _productRepository = new Mock<IProductRepository>();
        _currencyExchangeService = new Mock<ICurrencyExchangeService>();
        _service = new ProductService(_productRepository.Object, _currencyExchangeService.Object);
    }

    [Fact]
    public async Task GetProducts_ThrowsArgumentException_WhenPageSizeIsZero()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetProducts(0, 0, Currency.Gbp));
        Assert.Equal("pageSize", ex.ParamName);
    }

    [Fact]
    public async Task GetProducts_ThrowsArgumentException_WhenPageSizeIsNegative()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetProducts(0, -1, Currency.Gbp));
        Assert.Equal("pageSize", ex.ParamName);
    }

    [Fact]
    public async Task GetProducts_ThrowsArgumentException_WhenPageStartIsNegative()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetProducts(-1, 5, Currency.Gbp));
        Assert.Equal("pageStart", ex.ParamName);
    }

    [Fact]
    public async Task GetProducts_CallsRepository_WithCorrectParameters()
    {
        // Arrange
        const int pageStart = 1;
        const int pageSize = 3;

        _productRepository
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Products);

        _currencyExchangeService
            .Setup(x => x.GetExchangeRate(It.IsAny<Currency>()))
            .Returns(("£", 1m));

        // Act
        await _service.GetProducts(pageStart, pageSize, Currency.Gbp);

        // Assert
        _productRepository.Verify(x => x.GetProducts(pageStart, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmptyList_WhenRepositoryReturnsNoProducts()
    {
        // Arrange
        _productRepository
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Product>());

        _currencyExchangeService
            .Setup(x => x.GetExchangeRate(It.IsAny<Currency>()))
            .Returns(("£", 1m));

        // Act
        var result = await _service.GetProducts(0, 5, Currency.Gbp);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProducts_ReturnsPriceInPounds_WhenCurrencyIsGbp()
    {
        // Arrange
        _productRepository
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Products);

        _currencyExchangeService
            .Setup(x => x.GetExchangeRate(Currency.Gbp))
            .Returns(("£", 1m));

        // Act
        var result = await _service.GetProducts(0, 5, Currency.Gbp);
        var products = result.ToList();

        // Assert
        Assert.Equal("£1.00", products[0].FormattedPrice);
        Assert.Equal(1m, products[0].RawPrice);
    }

    [Fact]
    public async Task GetProducts_ReturnsCorrectProductCount_WhenCurrencyIsGbp()
    {
        // Arrange
        _productRepository
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Products);

        _currencyExchangeService
            .Setup(x => x.GetExchangeRate(Currency.Gbp))
            .Returns(("£", 1m));

        // Act
        var result = await _service.GetProducts(0, 5, Currency.Gbp);

        // Assert
        Assert.Equal(Products.Count, result.Count());
    }

    [Fact]
    public async Task GetProducts_ReturnsPriceInEuros_WhenCurrencyIsEur()
    {
        // Arrange
        _productRepository
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Products);

        _currencyExchangeService
            .Setup(x => x.GetExchangeRate(Currency.Eur))
            .Returns(("€", 1.11m));

        // Act
        var result = await _service.GetProducts(0, 5, Currency.Eur);
        var products = result.ToList();

        // Assert
        Assert.Equal("€1.11", products[0].FormattedPrice);
        Assert.Equal(1.11m, products[0].RawPrice);
    }

    [Fact]
    public async Task GetProducts_AppliesEuroRate_ToAllProducts()
    {
        // Arrange
        _productRepository
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Products);

        _currencyExchangeService
            .Setup(x => x.GetExchangeRate(Currency.Eur))
            .Returns(("€", 1.11m));

        // Act
        var result = await _service.GetProducts(0, 5, Currency.Eur);
        var products = result.ToList();

        // Assert
        Assert.Equal("€1.11", products[0].FormattedPrice);
        Assert.Equal("€1.22", products[1].FormattedPrice);
        Assert.Equal("€1.33", products[2].FormattedPrice);
    }

    [Fact]
    public async Task GetProducts_MapsProductNames_Correctly()
    {
        // Arrange
        _productRepository
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Products);

        _currencyExchangeService
            .Setup(x => x.GetExchangeRate(It.IsAny<Currency>()))
            .Returns(("£", 1m));

        // Act
        var result = await _service.GetProducts(0, 5, Currency.Gbp);
        var products = result.ToList();

        // Assert
        Assert.Equal("Sausage Roll", products[0].Name);
        Assert.Equal("Vegan Sausage Roll", products[1].Name);
        Assert.Equal("Steak Bake", products[2].Name);
    }
}