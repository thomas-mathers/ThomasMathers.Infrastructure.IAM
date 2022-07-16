using MediatR;
using Microsoft.AspNetCore.Identity;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Notifications;
using ThomasMathers.Infrastructure.IAM.Responses;

namespace ThomasMathers.Infrastructure.IAM.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(string userName, string password);
        Task<ChangePasswordResponse> ChangePassword(string userName, string currentPassword, string token, string newPassword);
        Task<string> ResetPassword(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IMediator _mediator;

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager, IAccessTokenGenerator accessTokenGenerator, IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accessTokenGenerator = accessTokenGenerator;
            _mediator = mediator;
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

        public async Task<string> ResetPassword(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _mediator.Publish(new ResetPasswordNotification
            {
                User = user,
                Token = token,
            });

            return token;
        }
    }
}
