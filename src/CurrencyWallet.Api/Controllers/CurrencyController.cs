#region Usings

using CurrencyWallet.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace CurrencyWallet.Api.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrencyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetWAvaiableCurrencies()
        {
            var query = new GetAvaiableCurrenciesQuery();
            return await _mediator.Send(query);
        }
    }
}