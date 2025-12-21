namespace WebApi1App.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebApi1App.Models;
using System.Net.Http.Json;

public class ProfilesController : Controller
{
    private readonly IHttpClientFactory httpClientFactory;

    public ProfilesController(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new ProfilesPageViewModel();
        await this.LoadProfiles(model);
        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Create")] CreateProfileFormModel model)
    {
        if (!this.ModelState.IsValid)
        {
            var page = new ProfilesPageViewModel { Create = model };
            await this.LoadProfiles(page);
            return this.View("Index", page);
        }

        try
        {
            var httpClient = this.httpClientFactory.CreateClient("ProfileApi");
            var response = await httpClient.PostAsJsonAsync("/api/profiles", new ProfileApiDto
            {
                FullName = model.FullName,
                Email = model.Email,
            });

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            var page = new ProfilesPageViewModel { Create = model, ApiError = ex.Message };
            await this.LoadProfiles(page);
            return this.View("Index", page);
        }

        return this.RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateProfileFormModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction("Index");
        }

        try
        {
            var httpClient = this.httpClientFactory.CreateClient("ProfileApi");
            var response = await httpClient.PutAsJsonAsync($"/api/profiles/{model.Id}", new ProfileApiDto
            {
                Id = model.Id,
                FullName = model.FullName,
                Email = model.Email,
            });

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            var page = new ProfilesPageViewModel
            {
                ApiError = ex.Message,
            };

            await this.LoadProfiles(page);
            return this.View("Index", page);
        }

        return this.RedirectToAction("Index");
    }

    private async Task LoadProfiles(ProfilesPageViewModel model)
    {
        try
        {
            var httpClient = this.httpClientFactory.CreateClient("ProfileApi");
            var profiles = await httpClient.GetFromJsonAsync<List<ProfileApiDto>>("/api/profiles");
            model.Profiles = profiles ?? new List<ProfileApiDto>();
            if (string.IsNullOrWhiteSpace(model.ApiError))
            {
                model.ApiError = null;
            }
        }
        catch (Exception ex)
        {
            if (string.IsNullOrWhiteSpace(model.ApiError))
            {
                model.ApiError = ex.Message;
            }
            model.Profiles = new List<ProfileApiDto>();
        }
    }
}
