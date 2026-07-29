namespace PeopleHub.Api.People;

public class Socio
{
    public int PersonId { get; set; }              // PK + FK
    public Person? Person { get; set; }

    public bool Ativo { get; set; } = true;
    public DateTime Desde { get; set; } = DateTime.UtcNow;
}