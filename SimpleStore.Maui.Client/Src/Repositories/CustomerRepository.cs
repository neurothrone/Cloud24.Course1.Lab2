using SimpleStore.Core.DTOs;
using SimpleStore.Core.Interfaces;
using SimpleStore.Core.Models;
using SimpleStore.Core.Utils;

namespace SimpleStore.Maui.Client.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IFileService _fileService;

    public CustomerRepository(IFileService fileService)
    {
        _fileService = fileService;
    }

    // NOTE: File is created here on:
    // - macOS:
    // /Users/username/Library/Containers/com.companyname.simplestore.maui.client/Data/Library/customers.json
    // - windows:
    private static readonly string CustomerFilePath = Path.Combine(FileSystem.AppDataDirectory, "customers.json");

    public void SaveCustomers(List<Customer> customers)
    {
        var customersData = customers
            .Select(c => c.ToCustomerDto())
            .ToList();

        _ = _fileService.WriteToJsonFile(CustomerFilePath, customersData);
    }

    public List<Customer> GetAllCustomers()
    {
        var customers = _fileService.ReadFromJsonFile<List<CustomerDto>>(CustomerFilePath);
        if (customers is null)
            return [];

        return customers
            .Select(c => c.ToCustomer())
            .ToList();
    }
}