using SimpleStore.Core.Enums;
using SimpleStore.Core.Models;

namespace SimpleStore.Core.Factories;

public static class CustomerFactory
{
    public static Customer CreateCustomer(
        string username,
        string password,
        double totalSpent,
        Membership membership)
    {
        return membership switch
        {
            Membership.Gold => new GoldCustomer(username, password, totalSpent),
            Membership.Silver => new SilverCustomer(username, password, totalSpent),
            Membership.Bronze => new BronzeCustomer(username, password, totalSpent),
            _ => throw new ArgumentOutOfRangeException(nameof(membership), membership, null)
        };
    }

    public static Customer CreateCustomer(Customer customer)
    {
        return customer.TotalSpent switch
        {
            >= 100 => new GoldCustomer(customer.Username, customer.Password, customer.TotalSpent),
            >= 50 => new SilverCustomer(customer.Username, customer.Password, customer.TotalSpent),
            _ => new BronzeCustomer(customer.Username, customer.Password, customer.TotalSpent)
        };
    }
}