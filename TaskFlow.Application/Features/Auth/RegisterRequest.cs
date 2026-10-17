namespace TaskFlow.Application.Features.Auth
{
    public record RegisterRequest(string FullName, string Email, string Password);
}