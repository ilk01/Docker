namespace WebApi1App.Models;

public class ProfilesPageViewModel
{
    public CreateProfileFormModel Create { get; set; } = new();

    public List<ProfileApiDto> Profiles { get; set; } = new();

    public string? ApiError { get; set; }
}
