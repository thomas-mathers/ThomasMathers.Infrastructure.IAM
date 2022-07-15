using ThomasMathers.Common.IAM.Settings;
using ThomasMathers.Common.IAM.Tests.MockObjects;
using Xunit;

namespace ThomasMathers.Common.IAM.Tests
{
    public class PasswordValidatorTests
    {
        private readonly PasswordValidator _sut;

        public PasswordValidatorTests()
        {
            var passwordSettings = new PasswordSettings { };

            _sut = new PasswordValidator(passwordSettings);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("aB(3f", false)]
        [InlineData("ab(3ef", false)]
        [InlineData("AB(3EF", false)]
        [InlineData("ab(Def", false)]
        [InlineData("aBc3ef", false)]
        [InlineData("aB(3ef", true)]
        public void Validate_ReturnsExpectedResult(string password, bool isValid)
        {
            var result = _sut.Validate(password);
            Assert.Equal(isValid, result.IsValid);
        }
    }
}
