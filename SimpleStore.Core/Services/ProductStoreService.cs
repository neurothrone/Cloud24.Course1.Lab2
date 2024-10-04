using SimpleStore.Core.Models;

namespace SimpleStore.Core.Services;

public class ProductStoreService
{
    public Product[] GetAvailableProducts() =>
    [
        new()
        {
            Name = "Apples, 1kg",
            Price = 15,
            Quantity = 20
        },
        new()
        {
            Name = "Bananas, 1kg",
            Price = 30,
            Quantity = 10
        },
        new()
        {
            Name = "Milk, 1L",
            Price = 22,
            Quantity = 15
        }
    ];
}