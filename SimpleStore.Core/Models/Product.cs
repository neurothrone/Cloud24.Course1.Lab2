namespace SimpleStore.Core.Models;

public class Product
{
    public required string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}