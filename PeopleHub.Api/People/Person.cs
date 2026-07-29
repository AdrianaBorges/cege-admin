namespace PeopleHub.Api.People;

public class Person
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Cpf { get; set; } = "";
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; }
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }

    public int TipoPessoaId { get; set; }
    public TipoPessoa? TipoPessoa { get; set; }
}