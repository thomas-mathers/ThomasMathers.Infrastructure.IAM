using FluentValidation;
using ThomasMathers.Common.IAM.Extensions;
using ThomasMathers.Common.IAM.Settings;

namespace ThomasMathers.Common.IAM.Tests.MockObjects
{
    internal class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator(PasswordSettings settings)
        {
            RuleFor(x => x).Password(settings);
        }
    }
}
