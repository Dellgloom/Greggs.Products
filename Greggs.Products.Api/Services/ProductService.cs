using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Repositories.Interfaces;
using Greggs.Products.Api.Services.Interfaces;

namespace Greggs.Products.Api.Services;

public class ProductService :IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICurrencyExchangeService _currencyExchangeService;

    public ProductService(IProductRepository productRepository, ICurrencyExchangeService currencyExchangeService)
    {
        _productRepository = productRepository;
        _currencyExchangeService = currencyExchangeService;
    }
    
    public async Task<IEnumerable<ProductResponse>> GetProducts(int pageStart, int pageSize, Currency currency)
    {
        var products = await _productRepository.GetProducts(pageStart, pageSize);
        
        var (symbol, rate) = _currencyExchangeService.GetExchangeRate(currency);

        return products.Select(x => new ProductResponse
        {
            Name = x.Name,
            Price = $"{symbol}{x.PriceInPounds * rate:0.00}"
        });
    }
}