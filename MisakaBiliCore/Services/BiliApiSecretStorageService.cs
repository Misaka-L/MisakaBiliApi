using System.Net;
using System.Text.Json;
using MisakaBiliCore.Models;

namespace MisakaBiliCore.Services;

public class BiliApiSecretStorageService
{
    public CookieContainer CookieContainer { get; } = new ();
    public string? RefreshToken { get; set; }


    public string? WbiImgKey { get; set; }
    public string? WbiSubKey { get; set; }

    public async Task SaveSecrets()
    {
        var containerData =
            JsonSerializer.Serialize(new SecretStorageContainer(CookieContainer.GetAllCookies().ToArray(),
                RefreshToken));

        await File.WriteAllTextAsync("data/secrets.json", containerData);
    }

    public async Task LoadSecrets()
    {
        // This is the most effective way I found to clear the CookieContainer
        foreach (var cookie in CookieContainer.GetAllCookies().ToArray())
        {
            cookie.Expired = true;
        }

        RefreshToken = null;

        if (!File.Exists("data/secrets.json"))
        {
            return;
        }

        var containerDataJson = await File.ReadAllTextAsync("data/secrets.json");
        var containerData = JsonSerializer.Deserialize<SecretStorageContainer>(containerDataJson);

        if (containerData is null)
            return;

        foreach (var cookie in containerData.Cookies)
        {
            CookieContainer.Add(cookie);
        }

        RefreshToken = containerData.RefreshToken;
    }
}
