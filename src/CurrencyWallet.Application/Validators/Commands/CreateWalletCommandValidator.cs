#region Usings

using CurrencyWallet.Application.Commands;
using FluentValidation;

#endregion

namespace CurrencyWallet.Application.Validators.Commands
{
    public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
    {
        public CreateWalletCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Wallet name is required.")
                .MaximumLength(50).WithMessage("Wallet name cannot exceed 50 characters.");
        }
    }
}
