using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Repositories.Interfaces;
using Greggs.Products.Api.Services.Interfaces;

namespace Greggs.Products.Api.Services;

public class ProductService : IProductService
{
    private readonly ICurrencyExchangeService _currencyExchangeService;
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository, ICurrencyExchangeService currencyExchangeService)
    {
        _productRepository = productRepository;
        _currencyExchangeService = currencyExchangeService;
    }

    public async Task<IEnumerable<ProductResponse>> GetProducts(int pageStart, int pageSize, Currency currency)
    {
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));

        if (pageStart < 0)
            throw new ArgumentException("Page start must be 0 or greater.", nameof(pageStart));

        var products = await _productRepository.GetProducts(pageStart, pageSize);

        var (symbol, rate) = _currencyExchangeService.GetExchangeRate(currency);

        return products.Select(x => new ProductResponse
        {
            Name = x.Name,
            FormattedPrice = $"{symbol}{x.PriceInPounds * rate:0.00}",
            RawPrice = Math.Round(x.PriceInPounds * rate, 2)
        });
    }
}