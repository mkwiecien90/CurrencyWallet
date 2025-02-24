#region Usings

using CurrencyWallet.Application.Commands;
using CurrencyWallet.Application.DTOs;
using CurrencyWallet.Application.Queries;
using CurrencyWallet.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace CurrencyWallet.Api.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<Guid> CreateWallet([FromBody] CreateWalletCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet("{id}")]
        public async Task<WalletViewModel> GetWallet([FromQuery] GetWalletQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpGet]
        public async Task<IEnumerable<WalletViewModel>> GetWallets()
        {
            var query = new GetAllWalletsQuery();
            return await _mediator.Send(query);
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> DepositToWallet(Guid id, [FromBody] DepositCommand command)
        {
            command.WalletId = id;
            await _mediator.Send(command);
            return Ok("Deposit successful.");
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> WithdrawFromWallet(Guid id, [FromBody] WithdrawCommand command)
        {
            command.WalletId = id;
            await _mediator.Send(command);
            return Ok("Withdrawal successful.");
        }

        [HttpPost("{id}/convert")]
        public async Task<IActionResult> ConvertCurrency(Guid id, [FromBody] ConvertCurrencyCommand command)
        {
            await _mediator.Send(command);
            return Ok("Currency conversion successful.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet([FromQuery] DeleteWalletCommand command)
        {
            await _mediator.Send(command);
            return Ok("Wallet is deleted.");
        }
    }
}