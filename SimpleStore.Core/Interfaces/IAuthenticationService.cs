using SimpleStore.Core.Models;

namespace SimpleStore.Core.Interfaces;

public interface IAuthenticationService
{
    Customer? SignIn(string username, string password);

    Customer? SignUp(string username, string password);
    void UpdateCustomer(Customer customer);
}