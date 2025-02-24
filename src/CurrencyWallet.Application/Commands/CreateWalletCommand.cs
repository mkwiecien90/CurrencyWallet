#region Usings

using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class CreateWalletCommand : IRequest<Guid>
    {
        public string Name { get; }

        public CreateWalletCommand(string name)
        {
            Name = name;
        }
    }
}
