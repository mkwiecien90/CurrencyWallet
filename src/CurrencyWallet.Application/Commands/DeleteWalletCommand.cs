#region Usings

using System.Text.Json.Serialization;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class DeleteWalletCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid WalletId { get; }

        public DeleteWalletCommand(Guid walletId)
        {
            WalletId = walletId;
        }
    }
}
