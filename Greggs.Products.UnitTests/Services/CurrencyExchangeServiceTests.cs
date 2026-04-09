using System;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests.Services;

public class CurrencyExchangeServiceTests
{
    private readonly CurrencyExchangeService _currencyExchangeService;
    private readonly Mock<ILogger<CurrencyExchangeService>> _logger;

    public CurrencyExchangeServiceTests()
    {
        _logger = new Mock<ILogger<CurrencyExchangeService>>();
        _currencyExchangeService = new CurrencyExchangeService(_logger.Object);
    }

    [Fact]
    public void GetExchangeRate_ReturnsGbpSymbol_WhenCurrencyIsGbp()
    {
        // Act
        var (symbol, _) = _currencyExchangeService.GetExchangeRate(Currency.Gbp);

        // Assert
        Assert.Equal("£", symbol);
    }

    [Fact]
    public void GetExchangeRate_ReturnsGbpRate_WhenCurrencyIsGbp()
    {
        // Act
        var (_, rate) = _currencyExchangeService.GetExchangeRate(Currency.Gbp);

        // Assert
        Assert.Equal(1m, rate);
    }

    [Fact]
    public void GetExchangeRate_ReturnsEurSymbol_WhenCurrencyIsEur()
    {
        // Act
        var (symbol, _) = _currencyExchangeService.GetExchangeRate(Currency.Eur);

        // Assert
        Assert.Equal("€", symbol);
    }

    [Fact]
    public void GetExchangeRate_ReturnsEurRate_WhenCurrencyIsEur()
    {
        // Act
        var (_, rate) = _currencyExchangeService.GetExchangeRate(Currency.Eur);

        // Assert
        Assert.Equal(1.11m, rate);
    }

    [Fact]
    public void GetExchangeRate_ThrowsNotSupportedException_WhenCurrencyIsNotSupported()
    {
        // Act & Assert
        Assert.Throws<NotSupportedException>(() => _currencyExchangeService.GetExchangeRate((Currency)99));
    }

    [Fact]
    public void GetExchangeRate_ExceptionMessage_ContainsCurrencyName()
    {
        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() => _currencyExchangeService.GetExchangeRate((Currency)99));
        Assert.Contains("is not a supported currency type", ex.Message);
    }

    [Fact]
    public void GetExchangeRate_LogsWarning_WhenCurrencyIsNotSupported()
    {
        // Act
        Assert.Throws<NotSupportedException>(() => _currencyExchangeService.GetExchangeRate((Currency)99));

        // Assert
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unsupported currency requested")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}