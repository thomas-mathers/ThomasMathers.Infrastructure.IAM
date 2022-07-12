using Microsoft.AspNetCore.Identity;
using ThomasMathers.Common.IAM.Data;
using ThomasMathers.Common.IAM.Responses;

namespace ThomasMathers.Common.IAM.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(string userName, string password);
        Task<ChangePasswordResponse> ChangePassword(string userName, string currentPassword, string token, string newPassword);
        Task<string> GeneratePasswordResetToken(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager, IAccessTokenGenerator accessTokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accessTokenGenerator = accessTokenGenerator;
        }

        public async Task<LoginResponse> Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return new NotFoundResponse();
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, true);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return new UserLockedOutResponse();
                }

                if (signInResult.RequiresTwoFactor)
                {
                    return new LoginRequiresTwoFactorResponse();
                }

                if (signInResult.IsNotAllowed)
                {
                    return new LoginIsNotAllowedResponse();
                }

                return new LoginFailureResponse();
            }
            
            var claims = await _userManager.GetClaimsAsync(user);

            return new LoginSuccessResponse(user.Id, user.UserName, user.Email, _accessTokenGenerator.GenerateAccessToken(claims));
        }

        public async Task<ChangePasswordResponse> ChangePassword(string userName, string currentPassword, string token, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return new NotFoundResponse();
            }

            if (!string.IsNullOrEmpty(currentPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(
                    user,
                    currentPassword,
                    newPassword);

                if (!changePasswordResult.Succeeded)
                {
                    return new IdentityErrorResponse(changePasswordResult.Errors);
                }

                return new ChangePasswordSuccessResponse();
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!resetPasswordResult.Succeeded)
            {
                return new IdentityErrorResponse(resetPasswordResult.Errors);
            }

            return new ChangePasswordSuccessResponse();
        }

        public async Task<string> GeneratePasswordResetToken(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}
