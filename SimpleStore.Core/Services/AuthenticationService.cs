using SimpleStore.Core.Interfaces;
using SimpleStore.Core.Models;

namespace SimpleStore.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly List<Customer> _customers = [];

    public AuthenticationService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
        LoadCustomersFromFile();
    }

    private Customer[] GetPredefinedCustomers() =>
    [
        new BronzeCustomer("Knatte", "123", 0d),
        new SilverCustomer("Fnatte", "321", 50d),
        new GoldCustomer("Tjatte", "213", 100d)
    ];

    private void LoadCustomersFromFile()
    {
        var savedCustomers = _customerRepository.GetAllCustomers();
        _customers.AddRange(savedCustomers);

        foreach (var predefinedCustomer in GetPredefinedCustomers())
        {
            if (!_customers.Exists(c => c.Username.Equals(predefinedCustomer.Username, StringComparison.Ordinal)))
                _customers.Add(predefinedCustomer);
        }
    }

    public void SaveRegisteredCustomersToFile()
    {
        _customerRepository.SaveCustomers(_customers);
    }

    #region IAuthenticationService

    public Customer? SignIn(string username, string password)
    {
        return _customers.FirstOrDefault(c =>
            c.Username.Equals(username) &&
            c.IsPasswordValid(password));
    }

    public Customer? SignUp(string username, string password)
    {
        if (_customers.Exists(c => c.Username.Equals(username, StringComparison.Ordinal)))
            return null;

        var newCustomer = new BronzeCustomer(username, password, 0d);
        _customers.Add(newCustomer);
        SaveRegisteredCustomersToFile();

        return newCustomer;
    }

    public void UpdateCustomer(Customer customer)
    {
        Customer? existingCustomer = _customers
            .FirstOrDefault(c => c.Username.Equals(customer.Username, StringComparison.Ordinal));
        if (existingCustomer is null)
            return;

        var index = _customers.IndexOf(existingCustomer);
        _customers[index] = customer;

        SaveRegisteredCustomersToFile();
    }

    #endregion
}