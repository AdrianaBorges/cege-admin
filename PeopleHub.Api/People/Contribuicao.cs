namespace PeopleHub.Api.People;

public class Contribuicao
{
    public long Id { get; set; }

    public int PersonId { get; set; }
    public Person? Person { get; set; }

    public int CompetenciaAno { get; set; }   // ex: 2026
    public int CompetenciaMes { get; set; }   // 1..12

    public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
    public decimal Valor { get; set; }        // > 0

    public string? Observacao { get; set; }
}