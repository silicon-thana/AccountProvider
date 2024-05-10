﻿using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class UserAddress
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string AddressType { get; set; } = null!;
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;

}