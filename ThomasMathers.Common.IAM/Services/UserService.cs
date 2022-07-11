using Microsoft.AspNetCore.Identity;
using ThomasMathers.Common.IAM.Data;
using ThomasMathers.Common.IAM.Requests;
using ThomasMathers.Common.IAM.Responses;

namespace ThomasMathers.Common.IAM.Services
{
    public interface IUserService
    {
        Task<User> GetUserByUserName(string userName);
        Task<RegisterResponse> Register(RegisterRequest registerRequest);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<User> GetUserByUserName(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            var user = new User
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email
            };

            var createResult = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!createResult.Succeeded)
            {
                return new IdentityErrorResponse(createResult.Errors);
            }

            return new RegisterSuccessResponse(user);
        }
    }
}
