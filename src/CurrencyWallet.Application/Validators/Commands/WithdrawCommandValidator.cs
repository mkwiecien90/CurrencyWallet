#region Usings

using CurrencyWallet.Application.Cache;
using CurrencyWallet.Application.Commands;
using FluentValidation;

#endregion

namespace CurrencyWallet.Application.Validators.Commands
{
    public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
    {
        public WithdrawCommandValidator(ICurrencyCache currencyCache)
        {
            RuleFor(x => x.WalletId)
             .NotEmpty().WithMessage("WalletId is required.")
             .Must(id => id != Guid.Empty).WithMessage("WalletId cannot be an empty GUID.");

            RuleFor(x => x.Currency)
            .NotNull()
            .WithMessage("The currency cannot be empty.")
            .Must(currency =>
            {
                var currencies = currencyCache.GetAvailableCurrencies();
                return currencies.Contains(currency);
            })
            .WithMessage("The specified currency is not available.");

            RuleFor(x => x.Amount)
             .NotNull()
             .WithMessage("The amount cannot be empty.")
             .GreaterThan(0)
             .WithMessage("The amount must be greater than zero.");
        }
    }
}
