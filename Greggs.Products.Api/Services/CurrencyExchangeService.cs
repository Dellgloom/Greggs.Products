using System;
using System.Collections.Generic;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Services;

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private static readonly Dictionary<Currency, (string Symbol, decimal Rate)> CurrencyLookup = new()
    {
        { Currency.Gbp, ("£", 1m) },
        { Currency.Eur, ("€", 1.11m) }
    };

    private readonly ILogger<CurrencyExchangeService> _logger;

    public CurrencyExchangeService(ILogger<CurrencyExchangeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Gets the symbol and exchange rate for the given currency.
    /// </summary>
    /// <param name="currency">The target currency</param>
    /// <returns>A tuple containing the symbol and the exchange rate multiplier for the currency.</returns>
    /// <exception cref="NotSupportedException">Thrown when a given currency is not supported.</exception>
    public (string Symbol, decimal Rate) GetExchangeRate(Currency currency)
    {
        if (!CurrencyLookup.TryGetValue(currency, out var value))
        {
            _logger.LogWarning("Unsupported currency requested: {Currency}", currency);
            throw new NotSupportedException($"{currency} is not a supported currency type.");
        }

        return value;
    }
}