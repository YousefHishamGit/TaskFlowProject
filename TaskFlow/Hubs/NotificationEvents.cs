namespace TaskFlow.Hubs
{
    public static class NotificationEvents
    {
        public const string TaskAssigned = "TaskAssigned";
        public const string TaskStatusChanged = "TaskStatusChanged";
        public static string ProjectGroup(Guid projectId) => $"project-{projectId}";
    }
}
