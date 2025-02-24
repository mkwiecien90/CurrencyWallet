#region Usings

using AutoMapper;
using CurrencyWallet.Application.DTOs;
using CurrencyWallet.Domain.Entities;

#endregion

namespace CurrencyWallet.Application.Mappings
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<Wallet, WalletViewModel>();
        }
    }
}
