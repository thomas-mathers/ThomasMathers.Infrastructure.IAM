using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Notifications;
using ThomasMathers.Infrastructure.IAM.Responses;

namespace ThomasMathers.Infrastructure.IAM.Services;

public interface IUserService
{
    Task<RegisterResponse> Register(User user, string roleName, string? password = null, CancellationToken cancellationToken = default);
    Task<User?> GetUserById(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default);
    Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default);
    Task DeleteUser(User user, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMediator _mediator;
    private readonly ILogger<UserService> _logger;

    public UserService
    (
        DatabaseContext databaseContext,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IMediator mediator,
        ILogger<UserService> logger
    )
    {
        _databaseContext = databaseContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<RegisterResponse> Register(User user, string roleName, string? password = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Registering {user}");

        using (var transaction = await _databaseContext.Database.BeginTransactionAsync(cancellationToken))
        {
            var createResult = password != null
                ? await _userManager.CreateAsync(user, password)
                : await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                _logger.LogWarning($"Failed to register {user}");
                return new IdentityErrorResponse(createResult.Errors);
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (roleExists == false)
            {
                return new NotFoundResponse();
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!addToRoleResult.Succeeded)
            {
                _logger.LogWarning($"Failed to add {user} to role {roleName}");
                return new IdentityErrorResponse(addToRoleResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);
        }

        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _logger.LogInformation($"User {user} has successfully registered");

        await _mediator.Publish(new UserRegisteredNotification
        {
            User = user,
            Token = emailConfirmationToken
        }, cancellationToken);

        return new RegisterSuccessResponse(user);
    }

    public Task<User?> GetUserById(Guid id, CancellationToken cancellationToken = default) => _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken: cancellationToken);

    public Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default) => _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default) => _databaseContext.Users.ToListAsync(cancellationToken);

    public Task DeleteUser(User user, CancellationToken cancellationToken = default)
    {
        _ = _databaseContext.Users.Remove(user);
        return _databaseContext.SaveChangesAsync(cancellationToken);
    }
}