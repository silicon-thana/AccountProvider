using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserAccount : IdentityUser
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set;}

    public string? AddressId { get; set; }
    public UserAddress? Address { get; set; }
}
