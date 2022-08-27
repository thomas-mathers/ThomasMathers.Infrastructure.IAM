using Microsoft.Extensions.Logging;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Responses;
using ThomasMathers.Infrastructure.IAM.Social.Services;

namespace ThomasMathers.Infrastructure.IAM.Services
{
    public interface ISocialAuthService
    {
        public Task<SocialLoginResponse> ExternalLogin(string provider, string userId, string accessToken, string roleName, CancellationToken cancellationToken = default);
    }

    public class SocialAuthService : ISocialAuthService
    {
        private readonly UserService _userService;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IReadOnlyDictionary<string, ISocialMediaProfileService> _socialMediaProfileServices;
        private readonly ILogger<SocialAuthService> _logger;

        public SocialAuthService
        (
            UserService userService,
            IAccessTokenGenerator accessTokenGenerator,
            IEnumerable<ISocialMediaProfileService> socialMediaProfileServices,
            ILogger<SocialAuthService> logger
        )
        {
            _userService = userService;
            _accessTokenGenerator = accessTokenGenerator;
            _socialMediaProfileServices = socialMediaProfileServices.ToDictionary(k => k.Name, v => v);
            _logger = logger;
        }

        public async Task<SocialLoginResponse> ExternalLogin(string provider, string userId, string accessToken, string roleName, CancellationToken cancellationToken = default)
        {
            if (!_socialMediaProfileServices.ContainsKey(provider))
            {
                return new SocialLoginProviderUnsupported();
            }

            var profileService = _socialMediaProfileServices[provider];

            var profile = await profileService.GetSocialMediaProfile(userId, accessToken, cancellationToken);

            var user = await _userService.GetUserByEmail(profile.Email, cancellationToken);

            if (user == null)
            {
                user = new User
                {
                    Email = profile.Email
                };

                user.Profiles.Add(new SocialMediaProfile
                {
                    UserId = user.Id,
                    Provider = provider,
                    ProviderUserId = userId,
                    Name = profile.Name,
                    Email = profile.Email,
                    ProfilePictureUrl = profile.ProfilePictureUrl
                });

                await _userService.Register(user, roleName, cancellationToken: cancellationToken);
            }

            var token = await _accessTokenGenerator.GenerateAccessToken(user);

            return new SocialLoginSuccessResponse(user, token);
        }
    }
}
