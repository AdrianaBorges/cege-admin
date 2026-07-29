using Microsoft.EntityFrameworkCore;

namespace PeopleHub.Api.People;

public class TipoPessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";

    public List<Person> Pessoas { get; set; } = new();
}