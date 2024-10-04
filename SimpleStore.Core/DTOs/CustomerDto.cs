using System.Text.Json.Serialization;
using SimpleStore.Core.Enums;

namespace SimpleStore.Core.DTOs;

public record CustomerDto(
    string Username,
    string Password,
    double TotalSpent,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    Membership Membership);