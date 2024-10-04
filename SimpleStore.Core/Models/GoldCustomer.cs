using SimpleStore.Core.Enums;

namespace SimpleStore.Core.Models;

public class GoldCustomer : Customer
{
    public GoldCustomer(
        string username,
        string password,
        double totalSpent) : base(username, password, totalSpent)
    {
        Membership = Membership.Gold;
    }
}