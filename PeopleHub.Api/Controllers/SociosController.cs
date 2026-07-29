using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleHub.Api.Data;
using PeopleHub.Api.People;

namespace PeopleHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SociosController : ControllerBase
{
    private readonly AppDbContext _db;
    public SociosController(AppDbContext db) => _db = db;

    [HttpPost("{personId:int}/ativar")]
    public async Task<IActionResult> Ativar(int personId, [FromBody] SocioAtivarDto dto)
    {
        var personExists = await _db.People.AnyAsync(p => p.Id == personId);
        if (!personExists) return NotFound("Pessoa não encontrada.");

        var socio = await _db.Socios.FirstOrDefaultAsync(s => s.PersonId == personId);

        var desde = dto.Desde ?? DateTime.UtcNow;

        if (socio == null)
        {
            socio = new Socio
            {
                PersonId = personId,
                Ativo = true,
                Desde = desde
            };
            _db.Socios.Add(socio);
        }
        else
        {
            socio.Ativo = true;

            // só atualiza "Desde" se você enviou no DTO
            if (dto.Desde.HasValue)
                socio.Desde = desde;
        }

        await _db.SaveChangesAsync();
        return Ok(new { socio.PersonId, socio.Ativo, socio.Desde });
    }

    [HttpPost("{personId:int}/desativar")]
    public async Task<IActionResult> Desativar(int personId)
    {
        var socio = await _db.Socios.FirstOrDefaultAsync(s => s.PersonId == personId);
        if (socio == null) return NotFound("Sócio não encontrado.");

        socio.Ativo = false;
        await _db.SaveChangesAsync();

        return Ok(new { socio.PersonId, socio.Ativo, socio.Desde });
    }
}