#region Usings

using CurrencyWallet.Application.Commands;
using FluentValidation;

#endregion

namespace CurrencyWallet.Application.Validators.Commands
{
    public class DeleteWalletCommandValidator : AbstractValidator<DeleteWalletCommand>
    {
        public DeleteWalletCommandValidator()
        {
            RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("WalletId is required.")
            .Must(id => id != Guid.Empty).WithMessage("WalletId cannot be an empty GUID.");
        }
    }
}
