using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Api.Services;
using StockTracker.Application.Commands.RegisterUser;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController :ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRepository _userRepository;
        private readonly Application.Interfaces.IPasswordHasher _passwordHasher;

        public AuthController(
            IMediator mediator,
            IJwtTokenService jwtTokenService,
            IUserRepository userRepository,
            Application.Interfaces.IPasswordHasher passwordHasher)
        {
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.DisplayName);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new { userId = result.Value, message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null)
                return Unauthorized(new { error = "Invalid email or password" });

            var isValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isValid)
                return Unauthorized(new { error = "Invalid email or password" });

            var token = _jwtTokenService.GenerateToken(user);

            return Ok(new
            {
                token,
                expiresIn = 3600,
                userId = user.Id,
                displayName = user.DisplayName
            });
        }
    }

    public record RegisterRequest(string Email, string Password, string DisplayName);
    public record LoginRequets(string Email, string Password);
}
