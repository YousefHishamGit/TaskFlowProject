namespace TaskFlow.Application.Features.Tasks
{
    public record CreateTaskRequest(
        string Title,
        string? Description,
        int Priority,
        DateTime? DueDate
    );
}