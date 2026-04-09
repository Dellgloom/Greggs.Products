using System;
using System.Net;
using System.Threading.Tasks;
using Greggs.Products.Api;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests.Integration;

public class ExceptionHandlingMiddlewareTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;
    private readonly Mock<IProductService> _productServiceMock;

    public ExceptionHandlingMiddlewareTests(WebApplicationFactory<Startup> factory)
    {
        _productServiceMock = new Mock<IProductService>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddSingleton(_productServiceMock.Object); });
        });
    }

    [Fact]
    public async Task Middleware_Returns400_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ThrowsAsync(new ArgumentException("Page size must be greater than 0."));

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/product");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Middleware_Returns400_WhenNotSupportedExceptionIsThrown()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ThrowsAsync(new NotSupportedException("Currency not supported."));

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/product");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Middleware_Returns500_WhenUnhandledExceptionIsThrown()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ThrowsAsync(new Exception("Something went wrong."));

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/product");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Middleware_ReturnsErrorMessage_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ThrowsAsync(new ArgumentException("Page size must be greater than 0."));

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/product");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Page size must be greater than 0.", content);
    }

    [Fact]
    public async Task Middleware_ReturnsGenericMessage_WhenUnhandledExceptionIsThrown()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Currency>()))
            .ThrowsAsync(new Exception("Something went wrong."));

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/product");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("An unexpected error occurred.", content);
    }
}