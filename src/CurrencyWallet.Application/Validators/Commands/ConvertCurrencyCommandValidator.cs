#region Usings

using CurrencyWallet.Application.Cache;
using CurrencyWallet.Application.Commands;
using FluentValidation;

#endregion

namespace CurrencyWallet.Application.Validators.Commands
{
    public class ConvertCurrencyCommandValidator : AbstractValidator<ConvertCurrencyCommand>
    {
        public ConvertCurrencyCommandValidator(ICurrencyCache currencyCache)
        {
            RuleFor(x => x.WalletId)
             .NotEmpty().WithMessage("WalletId is required.")
             .Must(id => id != Guid.Empty).WithMessage("WalletId cannot be an empty GUID.");

            RuleFor(x => x.FromCurrency)
            .NotNull()
            .WithMessage("The starting currency cannot be empty.")
            .Must(currency =>
            {
                var currencies = currencyCache.GetAvailableCurrencies();
                return currencies.Contains(currency);
            })
            .WithMessage("The specified starting currency is not available.");

            RuleFor(x => x.ToCurrency)
            .NotNull()
            .WithMessage("The destination currency cannot be empty.")
            .Must(currency =>
            {
                var currencies = currencyCache.GetAvailableCurrencies();
                return currencies.Contains(currency);
            })
            .WithMessage("The specified destination currency is not available.");

            RuleFor(x => x.Amount)
             .NotNull()
             .WithMessage("The amount cannot be empty.")
             .GreaterThan(0)
             .WithMessage("The amount must be greater than zero.");
        }
    }
}
