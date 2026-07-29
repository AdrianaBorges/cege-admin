namespace PeopleHub.Api.Auth.Dtos;
public record CreateUserDto(
    string Email,
    string Password,
    string Role // ex: "Admin" ou "User"
);
