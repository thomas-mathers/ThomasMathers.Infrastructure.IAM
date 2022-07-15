using FluentValidation;
using ThomasMathers.Common.IAM.Extensions;
using ThomasMathers.Common.IAM.Settings;

namespace ThomasMathers.Common.IAM.Tests.MockObjects
{
    internal class UsernameValidator : AbstractValidator<string>
    {
        public UsernameValidator(UserSettings userSettings)
        {
            RuleFor(x => x).Username(userSettings);
        }
    }
}
