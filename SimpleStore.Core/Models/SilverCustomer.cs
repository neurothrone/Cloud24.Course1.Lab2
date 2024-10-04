using SimpleStore.Core.Enums;

namespace SimpleStore.Core.Models;

public class SilverCustomer : Customer
{
    public SilverCustomer(
        string username,
        string password,
        double totalSpent) : base(username, password, totalSpent)
    {
        Membership = Membership.Silver;
    }
}