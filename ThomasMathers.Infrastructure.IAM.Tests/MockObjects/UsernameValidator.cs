using FluentValidation;

using ThomasMathers.Infrastructure.IAM.Extensions;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Tests.MockObjects;

internal class UsernameValidator : AbstractValidator<string>
{
    public UsernameValidator(UserSettings userSettings) => RuleFor(x => x).Username(userSettings);
}