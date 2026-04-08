using System.Collections.Generic;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts(int pageStart, int pageSize);
}