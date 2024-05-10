namespace AccountProvider.Models;

public class UserRegRequest
{

    public string? Firstname { get; set; }
    public string? Lastname { get; set; }

    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

}
