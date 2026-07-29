using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeopleHub.Api.Auth;
using PeopleHub.Api.Auth.Dtos;

namespace PeopleHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var role = dto.Role.Trim();

        // 1) valida role
        if (string.IsNullOrWhiteSpace(role))
            return BadRequest("Role é obrigatória.");

        // 2) garante que a role existe
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        // 3) evita duplicidade
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null)
            return Conflict("Usuário já existe.");

        // 4) cria usuário
        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // 5) atribui role
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
            return BadRequest(roleResult.Errors);

        return Ok(new { user.Id, user.Email, Role = role });
    }
}