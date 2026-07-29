namespace PeopleHub.Api.People;

public record PersonCreateDto(
    string Nome,
    string Cpf,
    string? Email,
    string? Telefone,
    DateTime? DataNascimento,
    bool Ativo,
    int TipoPessoaId
);

public record PersonUpdateDto(
    string Nome,
    string Cpf,
    string Email,
    string? Telefone,
    DateTime? DataNascimento,
    bool Ativo,
    int TipoPessoaId
);

public record PersonReadDto(
    int Id,
    string Nome,
    string Cpf,
    string Email,
    string? Telefone,
    DateTime? DataNascimento,
    bool Ativo,
    DateTime CreatedAt,

    int TipoPessoaId,
    string TipoPessoaNome,

    bool IsSocio,
    DateTime? SocioDesde
);