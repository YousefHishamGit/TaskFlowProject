namespace TaskFlow.Application.Features.Auth
{
    public record UserDto(Guid Id, string FullName, string Email, string Role);
}