using ClientOrder.Domain.Constants;
using ClientOrder.Services.Dtos;
using ClientOrder.Services.Exceptions;
using ClientOrder.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientOrder.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenService _tokenService;
    private readonly IUserResolverService _userResolverService;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        ITokenService tokenService,
        IUserResolverService userResolverService)
    {
        _authenticationService = authenticationService;
        _tokenService = tokenService;
        _userResolverService = userResolverService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
        if (loginDto is null)
        {
            throw new InvalidCredentialsException("Invalid credentials.");
        }

        var user = await _authenticationService.LoginAsync(loginDto.Username, loginDto.Password);
        var token = _tokenService.GenerateToken(user);

        return Ok(new LoginResponseDto
        {
            User = user,
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto registerUserDto)
    {
        var user = await _authenticationService.RegisterAsync(registerUserDto);
        return CreatedAtAction(
            nameof(UsersController.GetById),
            "Users",
            new { id = user.Id },
            user);
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = _userResolverService.GetById(),
            fullName = _userResolverService.GetFullName(),
            username = _userResolverService.GetUserName(),
            role = _userResolverService.GetRole()
        });
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("admin-check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult AdminCheck()
    {
        return Ok(new { message = "The API accepted the admin role" });
    }
}
