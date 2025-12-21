namespace WebApi1App.Models;

using System.ComponentModel.DataAnnotations;

public class CreateProfileFormModel
{
    [Required]
    [Display(Name = "Имя")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}
