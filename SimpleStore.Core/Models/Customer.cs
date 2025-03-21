using SimpleStore.Core.Enums;

namespace SimpleStore.Core.Models;

public abstract class Customer
{
    public string Username { get; }
    public string Password { get; }

    public double TotalSpent { get; set; }

    public Membership Membership { get; protected init; }

    protected Customer(
        string username,
        string password,
        double totalSpent)
    {
        Username = username;
        Password = password;
        TotalSpent = totalSpent;
    }

    public bool IsPasswordValid(string password) => Password.Equals(password, StringComparison.Ordinal);

    public override string ToString() => $"Username: {Username}, Password: {Password}";
}