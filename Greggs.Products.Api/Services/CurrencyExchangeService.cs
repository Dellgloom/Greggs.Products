using System;
using System.Collections.Generic;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services.Interfaces;

namespace Greggs.Products.Api.Services;

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private static readonly Dictionary<Currency, (string Symbol, decimal Rate)> CurrencyLookup = new()
    {
        { Currency.Gbp, ("£", 1m) },
        { Currency.Eur, ("€", 1.11m) }
    };
    
    public (string Symbol, decimal Rate) GetExchangeRate(Currency currency)
    {
        if (!CurrencyLookup.TryGetValue(currency, out var value))
            throw new NotSupportedException($"{currency} is not a supported currency type.");

        return value;
    }
}