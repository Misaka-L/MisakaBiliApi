using System.Net;

namespace MisakaBiliCore.Models;

public record SecretStorageContainer(Cookie[] Cookies, string? RefreshToken);
