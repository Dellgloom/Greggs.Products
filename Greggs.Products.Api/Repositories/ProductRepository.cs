using System.Collections.Generic;
using System.Threading.Tasks;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Repositories.Interfaces;

namespace Greggs.Products.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDataAccess<Product> _dataAccess;

    public ProductRepository(IDataAccess<Product> dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public Task<IEnumerable<Product>> GetProducts(int pageStart, int pageSize)
    {
        var products = _dataAccess.List(pageStart, pageSize);

        return Task.FromResult(products);
    }
}