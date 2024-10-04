using SimpleStore.Core.DTOs;
using SimpleStore.Core.Factories;
using SimpleStore.Core.Models;

namespace SimpleStore.Core.Utils;

public static class CustomerExtensions
{
    public static Customer ToCustomer(this CustomerDto dto) => CustomerFactory.CreateCustomer(
        dto.Username,
        dto.Password,
        dto.TotalSpent,
        dto.Membership
    );

    public static CustomerDto ToCustomerDto(this Customer customer) => new(
        customer.Username,
        customer.Password,
        customer.TotalSpent,
        customer.Membership
    );
}