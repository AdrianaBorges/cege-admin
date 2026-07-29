namespace PeopleHub.Api.People;

public record SocioAtivarDto(DateTime? Desde);

public record SocioReadDto(
    int PersonId,
    bool Ativo,
    DateTime Desde
);