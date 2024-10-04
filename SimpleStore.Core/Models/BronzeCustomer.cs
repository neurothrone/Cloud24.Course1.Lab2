using SimpleStore.Core.Enums;

namespace SimpleStore.Core.Models;

public class BronzeCustomer : Customer
{
    public BronzeCustomer(
        string username,
        string password,
        double totalSpent) : base(username, password, totalSpent)
    {
        Membership = Membership.Bronze;
    }
}