using SmartVault.Program.BusinessObjects;
using System.Threading.Tasks;

namespace SmartVault.Program.Interfaces
{
    public interface IOAuthService
    {
        Task<OAuthToken> RequestAccessTokenAsync(OAuthConfig config);
        Task<OAuthToken> RefreshAccessTokenAsync(OAuthConfig config, string refreshToken);
    }
}
