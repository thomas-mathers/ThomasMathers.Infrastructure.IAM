using FluentValidation;

using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder,
        UserSettings userSettings) => ruleBuilder
            .NotEmpty()
            .UsesAlphabet(userSettings.AllowedUserNameCharacters);

    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder,
        PasswordSettings passwordSettings) => ruleBuilder
            .MinimumLength(passwordSettings.RequiredLength)
            .WithMessage($"Must be at least {passwordSettings.RequiredLength} characters")
            .MinimumUniqueCharacters(passwordSettings.RequiredUniqueChars)
            .ContainsDigit(passwordSettings.RequireDigit)
            .ContainsUpper(passwordSettings.RequireUppercase)
            .ContainsLower(passwordSettings.RequireLowercase)
            .ContainsNonAlphanumeric(passwordSettings.RequireNonAlphanumeric);

    public static IRuleBuilder<T, string> UsesAlphabet<T>(this IRuleBuilder<T, string> ruleBuilder,
        HashSet<char> alphabet) => ruleBuilder.Must(s => s.All(alphabet.Contains))
            .WithMessage($"Must only use characters from {string.Join(",", alphabet)}");

    public static IRuleBuilder<T, string> UsesAlphabet<T>(this IRuleBuilder<T, string> ruleBuilder, string alphabet) => ruleBuilder.UsesAlphabet(new HashSet<char>(alphabet));


    public static IRuleBuilder<T, string> MinimumUniqueCharacters<T>(this IRuleBuilder<T, string> ruleBuilder,
        int minUniqueCharacters) => ruleBuilder.Must(s => s.Distinct().Count() >= minUniqueCharacters)
            .WithMessage($"Must have at least {minUniqueCharacters} unique characters");

    public static IRuleBuilder<T, string> ContainsDigit<T>(this IRuleBuilder<T, string> ruleBuilder, bool containsDigit) => !containsDigit ? ruleBuilder : ruleBuilder.Must(s => s.Any(char.IsDigit)).WithMessage("Must have at least one digit");

    public static IRuleBuilder<T, string> ContainsUpper<T>(this IRuleBuilder<T, string> ruleBuilder, bool containsUpper) => !containsUpper
            ? ruleBuilder
            : ruleBuilder.Must(s => s.Any(char.IsUpper)).WithMessage("Must have at least one uppercase letter");

    public static IRuleBuilder<T, string> ContainsLower<T>(this IRuleBuilder<T, string> ruleBuilder, bool containsLower) => !containsLower
            ? ruleBuilder
            : ruleBuilder.Must(s => s.Any(char.IsLower)).WithMessage("Must have at least one lowercase letter");

    public static IRuleBuilder<T, string> ContainsNonAlphanumeric<T>(this IRuleBuilder<T, string> ruleBuilder,
        bool containsNonAlphanumeric) => !containsNonAlphanumeric
            ? ruleBuilder
            : ruleBuilder.Must(s => s.Any(c => !char.IsLetterOrDigit(c)))
            .WithMessage("Must have at least one non alphanumeric letter");
}