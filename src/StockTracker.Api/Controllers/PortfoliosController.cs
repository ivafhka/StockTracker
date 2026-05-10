using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Application.Commands.AddPosition;
using StockTracker.Application.Commands.CreatePortfolio;
using StockTracker.Application.Queries.GetPortfolio;
using StockTracker.Application.Queries.GetUserPortfolios;

namespace StockTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PortfoliosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PortfoliosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid UserId => Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException());

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetUserPortfoliosQuery(UserId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetPortfolioQuery(id);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePortfolioRequest request)
        {
            var command = new CreatePortfolioCommand(UserId, request.Name, request.Description);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Value},new {id = result.Value });
        }

        [HttpPost("{id:guid}/positions")]
        public async Task<IActionResult> AddPosition(Guid id, [FromBody] AddPositionRequest request)
        {
            var command = new AddPositionCommand(id, request.Ticker, request.quantity, request.BuyPrice, request.Currency);
            var result = await _mediator.Send(command);

            if(result.IsFailure)
                return BadRequest(new {error = result.Error});
            
            return Ok(new {positionId = result.Value});
        }
    }

    public record CreatePortfolioRequest(string Name, string? Description);
    public record AddPositionRequest(string Ticker, decimal quantity, decimal BuyPrice, string Currency);
}
