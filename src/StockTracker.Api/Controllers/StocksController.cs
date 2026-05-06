using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Application.Queries.GetPriceHistory;

namespace StockTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StocksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StocksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{ticker}/history")]
        public async Task<IActionResult> GetHistory(
            string ticker,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var query = new GetPriceHistoryQuery(ticker, from, to);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
    }
}
