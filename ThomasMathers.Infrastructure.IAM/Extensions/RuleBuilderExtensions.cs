using FluentValidation;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder,
        UserSettings userSettings)
    {
        return ruleBuilder
            .NotEmpty()
            .UsesAlphabet(userSettings.AllowedUserNameCharacters);
    }

    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder,
        PasswordSettings passwordSettings)
    {
        return ruleBuilder
            .MinimumLength(passwordSettings.RequiredLength)
            .WithMessage($"Must be at least {passwordSettings.RequiredLength} characters")
            .MinimumUniqueCharacters(passwordSettings.RequiredUniqueChars)
            .ContainsDigit(passwordSettings.RequireDigit)
            .ContainsUpper(passwordSettings.RequireUppercase)
            .ContainsLower(passwordSettings.RequireLowercase)
            .ContainsNonAlphanumeric(passwordSettings.RequireNonAlphanumeric);
    }

    public static IRuleBuilder<T, string> UsesAlphabet<T>(this IRuleBuilder<T, string> ruleBuilder,
        HashSet<char> alphabet)
    {
        return ruleBuilder.Must(s => s.All(alphabet.Contains))
            .WithMessage($"Must only use characters from {string.Join(",", alphabet)}");
    }

    public static IRuleBuilder<T, string> UsesAlphabet<T>(this IRuleBuilder<T, string> ruleBuilder, string alphabet)
    {
        return ruleBuilder.UsesAlphabet(new HashSet<char>(alphabet));
    }


    public static IRuleBuilder<T, string> MinimumUniqueCharacters<T>(this IRuleBuilder<T, string> ruleBuilder,
        int minUniqueCharacters)
    {
        return ruleBuilder.Must(s => s.Distinct().Count() >= minUniqueCharacters)
            .WithMessage($"Must have at least {minUniqueCharacters} unique characters");
    }

    public static IRuleBuilder<T, string> ContainsDigit<T>(this IRuleBuilder<T, string> ruleBuilder, bool containsDigit)
    {
        if (!containsDigit) return ruleBuilder;

        return ruleBuilder.Must(s => s.Any(char.IsDigit)).WithMessage("Must have at least one digit");
    }

    public static IRuleBuilder<T, string> ContainsUpper<T>(this IRuleBuilder<T, string> ruleBuilder, bool containsUpper)
    {
        if (!containsUpper) return ruleBuilder;

        return ruleBuilder.Must(s => s.Any(char.IsUpper)).WithMessage("Must have at least one uppercase letter");
    }

    public static IRuleBuilder<T, string> ContainsLower<T>(this IRuleBuilder<T, string> ruleBuilder, bool containsLower)
    {
        if (!containsLower) return ruleBuilder;

        return ruleBuilder.Must(s => s.Any(char.IsLower)).WithMessage("Must have at least one lowercase letter");
    }

    public static IRuleBuilder<T, string> ContainsNonAlphanumeric<T>(this IRuleBuilder<T, string> ruleBuilder,
        bool containsNonAlphanumeric)
    {
        if (!containsNonAlphanumeric) return ruleBuilder;

        return ruleBuilder.Must(s => s.Any(c => !char.IsLetterOrDigit(c)))
            .WithMessage("Must have at least one non alphanumeric letter");
    }
}