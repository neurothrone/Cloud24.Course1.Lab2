using SimpleStore.Core.Models;

namespace SimpleStore.Core.Interfaces;

public interface ICustomerRepository
{
    void SaveCustomers(List<Customer> customers);
    List<Customer> GetAllCustomers();
}