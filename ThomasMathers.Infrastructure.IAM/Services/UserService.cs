using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Notifications;
using ThomasMathers.Infrastructure.IAM.Responses;

namespace ThomasMathers.Infrastructure.IAM.Services;

public interface IUserService
{
    Task<RegisterResponse> Register(User user, string password);
}

public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserService> _logger; 


    public UserService(UserManager<User> userManager, IMediator mediator, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<RegisterResponse> Register(User user, string password)
    {
        _logger.LogInformation($"Registering {user}");

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            _logger.LogWarning($"Failed to register {user}");
            return new IdentityErrorResponse(createResult.Errors);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _logger.LogInformation($"User {user} has successfully registered");

        await _mediator.Publish(new UserRegisteredNotification
        {
            User = user,
            Token = token
        });

        return new RegisterSuccessResponse(user);
    }
}