#region Usings

using CurrencyWallet.Application.Queries;
using FluentValidation;

#endregion

namespace CurrencyWallet.Application.Validators.Queries
{
    public class GetWalletQueryValidator : AbstractValidator<GetWalletQuery>
    {
        public GetWalletQueryValidator()
        {
            RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("WalletId is required.")
            .Must(id => id != Guid.Empty).WithMessage("WalletId cannot be an empty GUID.");
        }
    }
}
