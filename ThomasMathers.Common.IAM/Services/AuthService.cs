using Microsoft.AspNetCore.Identity;
using ThomasMathers.Common.IAM.Data;
using ThomasMathers.Common.IAM.Requests;
using ThomasMathers.Common.IAM.Responses;

namespace ThomasMathers.Common.IAM.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest changePasswordRequest);
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

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (user == null)
            {
                return new NotFoundResponse();
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, true);

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

        public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByNameAsync(changePasswordRequest.UserName);

            if (user == null)
            {
                return new NotFoundResponse();
            }

            if (!string.IsNullOrEmpty(changePasswordRequest.CurrentPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(
                    user,
                    changePasswordRequest.CurrentPassword,
                    changePasswordRequest.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    return new IdentityErrorResponse(changePasswordResult.Errors);
                }

                return new ChangePasswordSuccessResponse();
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, changePasswordRequest.Token, changePasswordRequest.NewPassword);

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
