using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Application.Commands.CreateAlert;
using StockTracker.Application.Queries.GetAlerts;
using StockTracker.Domain.Entities;
using System.Security.Claims;

namespace StockTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlertsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        private Guid UserId => Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException());


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAlertsQuery(UserId);
            var result = await _mediator.Send(query); ;

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAlertRequest request)
        {
            if (!Enum.TryParse<AlertDirection>(request.Direction, true, out var direction))
                return BadRequest(new { error = "Direction must be 'Above' or 'Below'" });

            var command = new CreateAlertCommand(
                UserId,
                request.Ticker,
                request.TargetPrice,
                request.Currency,
                direction);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new {alertId = result.Value});
        }
    }

    public record CreateAlertRequest(
        string Ticker,
        decimal TargetPrice,
        string Currency,
        string Direction);
}
