using System.Collections.Generic;
using System.Threading.Tasks;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Repositories;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests.Repositories;

public class ProductRepositoryTests
{
    private static readonly List<Product> Products = new()
    {
        new Product { Name = "Sausage Roll", PriceInPounds = 1m },
        new Product { Name = "Vegan Sausage Roll", PriceInPounds = 1.1m },
        new Product { Name = "Steak Bake", PriceInPounds = 1.2m }
    };

    private readonly Mock<IDataAccess<Product>> _dataAccess;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _dataAccess = new Mock<IDataAccess<Product>>();
        _repository = new ProductRepository(_dataAccess.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsProducts_WhenDataAccessReturnsProducts()
    {
        // Arrange
        _dataAccess
            .Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Products);

        // Act
        var result = await _repository.GetProducts(0, 5);

        // Assert
        Assert.Equal(Products, result);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmptyList_WhenDataAccessReturnsNoProducts()
    {
        // Arrange
        _dataAccess
            .Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new List<Product>());

        // Act
        var result = await _repository.GetProducts(0, 5);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProducts_CallsDataAccess_WithCorrectParameters()
    {
        // Arrange
        const int pageStart = 2;
        const int pageSize = 3;

        _dataAccess
            .Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Products);

        // Act
        await _repository.GetProducts(pageStart, pageSize);

        // Assert
        _dataAccess.Verify(x => x.List(pageStart, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetProducts_CallsDataAccess_ExactlyOnce()
    {
        // Arrange
        _dataAccess
            .Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Products);

        // Act
        await _repository.GetProducts(0, 5);

        // Assert
        _dataAccess.Verify(x => x.List(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }
}