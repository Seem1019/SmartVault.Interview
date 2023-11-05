using SmartVault.Program.BusinessObjects;
using SmartVault.Program.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartVault.Program.Services
{
    public class OAuthService : IOAuthService
    {
        public async Task<OAuthToken> RequestAccessTokenAsync(OAuthConfig config)
        {
            throw new NotImplementedException("Logic to request access to a new token...");
        }

        public async Task<OAuthToken> RefreshAccessTokenAsync(OAuthConfig config, string refreshToken)
        {

            throw new NotImplementedException("Logic to refresh token...");
        }
    }
}
