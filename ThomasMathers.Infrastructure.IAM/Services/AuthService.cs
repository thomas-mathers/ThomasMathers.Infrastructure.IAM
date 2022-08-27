using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Notifications;
using ThomasMathers.Infrastructure.IAM.Responses;
using ThomasMathers.Infrastructure.IAM.Social.Services;

namespace ThomasMathers.Infrastructure.IAM.Services;

public interface IAuthService
{
    Task<ConfirmEmailResponse> ConfirmEmail(string userName, string token);
    Task<LoginResponse> Login(string userName, string password);
    Task<ResetPasswordResponse> ResetPassword(string userName);
    Task<ChangePasswordResponse> ChangePassword(string userName, string currentPassword, string token,
        string newPassword);
}

public class AuthService : IAuthService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IMediator _mediator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        SignInManager<User> signInManager, 
        UserManager<User> userManager,
        IAccessTokenGenerator accessTokenGenerator,
        IMediator mediator,
        ILogger<AuthService> logger
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accessTokenGenerator = accessTokenGenerator;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<LoginResponse> Login(string userName, string password)
    {
        _logger.LogInformation($"Attempting to log user {userName} in");

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            _logger.LogWarning($"An attempt was made to login with username {userName} which does not exist");
            return new NotFoundResponse();
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, true);

        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
            {
                _logger.LogWarning($"An attempt was made to login with username {userName} which is currently locked out");
                return new UserLockedOutResponse();
            }

            if (signInResult.RequiresTwoFactor)
            {
                _logger.LogWarning($"An attempt was made to login with username {userName} which requires two factor authentication");
                return new LoginRequiresTwoFactorResponse();
            }

            if (signInResult.IsNotAllowed)
            {
                _logger.LogWarning($"An attempt was made to login with username {userName} which is not allowed two factor authentication");
                return new LoginIsNotAllowedResponse();
            }

            return new LoginFailureResponse();
        }

        _logger.LogInformation($"User {userName} has successfully logged in");

        var token = await _accessTokenGenerator.GenerateAccessToken(user);

        return new LoginSuccessResponse(user, token);
    }

    public async Task<ChangePasswordResponse> ChangePassword(string userName, string currentPassword, string token,
        string newPassword)
    {
        _logger.LogInformation($"Attempting to change user {userName} password");

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            _logger.LogWarning($"An attempt was made to change password of user {userName} which does not exist");
            return new NotFoundResponse();
        }

        if (!string.IsNullOrEmpty(currentPassword))
        {
            _logger.LogInformation($"Attempting to change user {userName} password using the current password as a token");

            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);

            if (!changePasswordResult.Succeeded)
            {
                _logger.LogWarning($"An attempt to change user {userName} password using the current password has failed");
                return new IdentityErrorResponse(changePasswordResult.Errors);
            }

            _logger.LogInformation($"User {userName} has successfully changed his password using his current passsword ");

            return new ChangePasswordSuccessResponse();
        }

        _logger.LogInformation($"Attempting to change user {userName} password using a password reset token");

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!resetPasswordResult.Succeeded)
        {
            _logger.LogWarning($"An attempt to change user {userName} password using password reset token has failed");
            return new IdentityErrorResponse(resetPasswordResult.Errors);
        }

        _logger.LogInformation($"User {userName} has successfully changed his password using supplied password reset token");

        return new ChangePasswordSuccessResponse();
    }

    public async Task<ResetPasswordResponse> ResetPassword(string userName)
    {
        _logger.LogInformation($"Attempting to reset user {userName} password");

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            _logger.LogWarning($"An attempt was made to reset password of user {userName} which does not exist");
            return new NotFoundResponse();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        _logger.LogWarning($"Successfully generated password reset token for user {userName}");

        await _mediator.Publish(new ResetPasswordNotification
        {
            User = user,
            Token = token
        });

        return new ResetPasswordSuccessResponse(token);
    }

    public async Task<ConfirmEmailResponse> ConfirmEmail(string userName, string token)
    {
        _logger.LogInformation($"Attempting to confirm email for user {userName}");

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            _logger.LogWarning($"An attempt was made to confirm email for {userName} which does not exist");
            return new NotFoundResponse();
        }

        var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);

        if (!confirmEmailResult.Succeeded)
        {
            _logger.LogWarning($"An attempt was confirm email for {userName} has failed");
            return new IdentityErrorResponse(confirmEmailResult.Errors);
        }

        _logger.LogWarning($"Successfully confirmed email for {userName}");

        return new ConfirmEmailSuccessResponse();
    }
}