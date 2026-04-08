using System.Collections.Generic;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetProducts(int pageStart, int pageSize, Currency currency);
}