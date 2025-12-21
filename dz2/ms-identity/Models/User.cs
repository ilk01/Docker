using System.ComponentModel.DataAnnotations;

namespace ms_identity.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string PasswordHash { get; set; }
}
