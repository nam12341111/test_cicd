using Microsoft.AspNetCore.Mvc;
using PTJ.Application.DTOs.Auth;
using PTJ.Application.Services;

namespace PTJ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var result = await _authService.LoginAsync(dto, ipAddress, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ipAddress, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var result = await _authService.RevokeTokenAsync(dto.RefreshToken, ipAddress, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
