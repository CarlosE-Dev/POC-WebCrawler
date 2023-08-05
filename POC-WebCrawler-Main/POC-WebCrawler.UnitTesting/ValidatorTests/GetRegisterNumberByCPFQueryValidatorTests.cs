using FluentAssertions;
using POC_WebCrawler.Application.CQRS.Inputs;
using POC_WebCrawler.Application.Validators;
using Xunit;

namespace POC_WebCrawler.UnitTesting.ValidatorTests
{
    public class GetRegisterNumberByCPFQueryValidatorTests
    {
        private readonly GetRegisterNumberByCPFQueryValidator _validator;

        public GetRegisterNumberByCPFQueryValidatorTests()
        {
            _validator = new GetRegisterNumberByCPFQueryValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CPFValidator_Should_Have_ValidationError_When_Empty(string cpf)
        {
            // Arrange
            var query = new GetRegisterNumberByCPFQuery(cpf);

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CustomerCPF");
        }

        [Theory]
        [InlineData("123.456.789-0")]
        [InlineData("12345678900")]
        public void CPFValidator_Should_Have_ValidationError_When_Invalid_Format(string cpf)
        {
            // Arrange
            var query = new GetRegisterNumberByCPFQuery(cpf);

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CustomerCPF");
        }

        [Theory]
        [InlineData("123.456.789-01")]
        [InlineData("987.654.321-09")]
        public void CPFValidator_Should_Proceed_When_Valid_Format(string cpf)
        {
            // Arrange
            var query = new GetRegisterNumberByCPFQuery(cpf);

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
