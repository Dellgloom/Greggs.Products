namespace Greggs.Products.Api.Models;

public class ProductResponse
{
    public string Name { get; set; }

    public string FormattedPrice { get; set; }

    public decimal RawPrice { get; set; }
}