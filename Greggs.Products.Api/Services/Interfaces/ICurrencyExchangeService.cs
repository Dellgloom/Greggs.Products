using System.Threading.Tasks;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.Services.Interfaces;

public interface ICurrencyExchangeService
{
    (string Symbol, decimal Rate) GetExchangeRate(Currency currency);
}