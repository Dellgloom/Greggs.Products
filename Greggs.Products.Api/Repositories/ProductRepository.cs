using System.Collections.Generic;
using System.Threading.Tasks;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Repositories;

public class ProductRepository :  IProductRepository
{
    private readonly IDataAccess<Product> _dataAccess;

    public ProductRepository(IDataAccess<Product> dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public Task<IEnumerable<Product>> GetProducts(int pageStart, int pageSize)
    {
        var products = _dataAccess.List(pageStart, pageSize);
        
        // I have used async throughout my layers as I wanted to simulate getting the data from a database, but I did not
        // want to touch the ProductAccess class. This would be changed to be async/await in a real database implementation.
        return Task.FromResult(products);
    }
    
}