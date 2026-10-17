namespace TaskFlow.Application.Features.Auth
{
    public record AuthResponse(string Token, UserDto User);
}