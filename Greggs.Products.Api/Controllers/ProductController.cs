using System.Collections.Generic;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _productService;

    public ProductController(ILogger<ProductController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    /// <summary>
    ///     Gets a list of products and their prices in the given currency.
    /// </summary>
    /// <param name="pageStart">The index to start the page from. Defaults to 0.</param>
    /// <param name="pageSize">The number of products to return. Defaults to 5.</param>
    /// <param name="currency">The currency in which prices are returned. Defaults to GBP.</param>
    /// <returns>A list of products with prices formatted in the given currency.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts(int pageStart = 0, int pageSize = 5,
        Currency currency = Currency.Gbp)
    {
        _logger.LogInformation(
            "GetProducts called with pageStart: {PageStart}, pageSize: {PageSize}, currency: {Currency}", pageStart,
            pageSize, currency);

        var products = await _productService.GetProducts(pageStart, pageSize, currency);

        return Ok(products);
    }
}