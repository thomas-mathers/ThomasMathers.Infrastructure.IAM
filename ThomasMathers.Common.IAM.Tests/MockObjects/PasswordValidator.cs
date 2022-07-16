using FluentValidation;
using ThomasMathers.Infrastructure.IAM.Extensions;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Tests.MockObjects
{
    internal class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator(PasswordSettings settings)
        {
            RuleFor(x => x).Password(settings);
        }
    }
}
