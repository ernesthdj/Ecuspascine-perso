using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

public class ApiSession : IApiSession
{
    public string? AccessToken { get; set; }
}
