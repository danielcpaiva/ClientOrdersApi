using ClientOrder.Domain.Constants;
using ClientOrder.Services.Dtos;
using ClientOrder.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientOrder.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserReadDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserReadDto>>> GetAll()
        => Ok(await _userService.GetAllAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType<UserReadDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserReadDto>> GetById(int id)
        => Ok(await _userService.GetByIdAsync(id));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ProducesResponseType<UserReadDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserReadDto>> Create(UserCreateDto request)
    {
        var user = await _userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, UserCreateEditDto request)
    {
        await _userService.UpdateAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:int}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRole(int id, ChangeRoleDto request)
    {
        await _userService.ChangeRoleAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}
