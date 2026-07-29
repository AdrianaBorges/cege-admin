using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleHub.Api.Common;
using PeopleHub.Api.Data;
using PeopleHub.Api.People;

namespace PeopleHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PeopleController : ControllerBase
{
    private readonly AppDbContext _db;

    public PeopleController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<PersonReadDto>>> GetAll()
    {
        var list = await _db.People
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => new PersonReadDto(
                p.Id,
                p.Nome,
                p.Cpf,
                p.Email,
                p.Telefone,
                p.DataNascimento,
                p.Ativo,
                p.CreatedAt,

                p.TipoPessoaId,
                p.TipoPessoa != null ? p.TipoPessoa.Nome : "",

                _db.Socios.Any(s => s.PersonId == p.Id && s.Ativo),
                _db.Socios
                    .Where(s => s.PersonId == p.Id && s.Ativo)
                    .Select(s => (DateTime?)s.Desde)
                    .FirstOrDefault()
            ))
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonReadDto>> GetById(int id)
    {
        var dto = await _db.People
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PersonReadDto(
                p.Id,
                p.Nome,
                p.Cpf,
                p.Email,
                p.Telefone,
                p.DataNascimento,
                p.Ativo,
                p.CreatedAt,

                p.TipoPessoaId,
                p.TipoPessoa != null ? p.TipoPessoa.Nome : "",

                _db.Socios.Any(s => s.PersonId == p.Id && s.Ativo),
                _db.Socios
                    .Where(s => s.PersonId == p.Id && s.Ativo)
                    .Select(s => (DateTime?)s.Desde)
                    .FirstOrDefault()
            ))
            .FirstOrDefaultAsync();

        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PersonReadDto>> Create([FromBody] PersonCreateDto dto)
    {
        var cpf = Cpf.OnlyDigits(dto.Cpf);

        if (!Cpf.IsValid(cpf))
            return BadRequest("CPF inválido.");

        var exists = await _db.People.AnyAsync(x => x.Cpf == cpf);
        if (exists)
            return Conflict("CPF já cadastrado.");

        // valida TipoPessoaId (pra não gravar FK inválida)
        var tipoOk = await _db.Set<TipoPessoa>().AnyAsync(t => t.Id == dto.TipoPessoaId);
        if (!tipoOk)
            return BadRequest("TipoPessoaId inválido.");

        var entity = new Person
        {
            Nome = dto.Nome.Trim(),
            Cpf = cpf,
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            Telefone = string.IsNullOrWhiteSpace(dto.Telefone) ? null : dto.Telefone.Trim(),
            DataNascimento = dto.DataNascimento,
            Ativo = dto.Ativo,
            TipoPessoaId = dto.TipoPessoaId
        };

        _db.People.Add(entity);
        await _db.SaveChangesAsync();

        // pega o nome do tipo (evita depender da navegação estar carregada)
        var tipoNome = await _db.Set<TipoPessoa>()
            .Where(t => t.Id == entity.TipoPessoaId)
            .Select(t => t.Nome)
            .FirstOrDefaultAsync() ?? "";

        var result = new PersonReadDto(
            entity.Id,
            entity.Nome,
            entity.Cpf,
            entity.Email,
            entity.Telefone,
            entity.DataNascimento,
            entity.Ativo,
            entity.CreatedAt,

            entity.TipoPessoaId,
            tipoNome,

            false,   // IsSocio
            null     // SocioDesde
        );

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonUpdateDto dto)
    {
        var entity = await _db.People.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        var cpf = Cpf.OnlyDigits(dto.Cpf);
        if (!Cpf.IsValid(cpf))
            return BadRequest("CPF inválido.");

        var cpfEmUsoPorOutro = await _db.People.AnyAsync(x => x.Cpf == cpf && x.Id != id);
        if (cpfEmUsoPorOutro)
            return Conflict("CPF já cadastrado para outra pessoa.");

        // Se seu PersonUpdateDto também tiver TipoPessoaId, valida e atualiza.
        // Se NÃO tiver, pode remover esse bloco.
        var tipoOk = await _db.Set<TipoPessoa>().AnyAsync(t => t.Id == dto.TipoPessoaId);
        if (!tipoOk)
            return BadRequest("TipoPessoaId inválido.");

        entity.Nome = dto.Nome.Trim();
        entity.Cpf = cpf;
        entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        entity.Telefone = string.IsNullOrWhiteSpace(dto.Telefone) ? null : dto.Telefone.Trim();
        entity.DataNascimento = dto.DataNascimento;
        entity.Ativo = dto.Ativo;
        entity.TipoPessoaId = dto.TipoPessoaId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.People.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        _db.People.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}