using FluentValidation;
using POC_WebCrawler.Application.CQRS.Inputs;
using System.Text.RegularExpressions;

namespace POC_WebCrawler.Application.Validators
{
    public class GetRegisterNumberByCPFQueryValidator : AbstractValidator<GetRegisterNumberByCPFQuery>
    {
        public GetRegisterNumberByCPFQueryValidator()
        {
            RuleFor(p => p.CustomerCPF)
                .NotEmpty()
                    .WithMessage("The field CPF is required.")
                .NotNull()
                    .WithMessage("The field CPF is required.")
                .Must(BeValid)
                    .WithMessage("Invalid CPF format.");
        }

        private bool BeValid(string cpf)
        {
            string cpfWithoutSymbols = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpfWithoutSymbols.Length != 11)
                return false;

            string cpfPattern = @"^\d{3}\.\d{3}\.\d{3}-\d{2}$";
            return Regex.IsMatch(cpf, cpfPattern);
        }
    }
}
