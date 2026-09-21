namespace Domain.Enums;

public static class TaskStatusExtensions
{
    public static bool CanTransitionTo(this TaskStatus currentStatus, TaskStatus newStatus) =>
        currentStatus == newStatus || (currentStatus, newStatus) switch
        {
            (TaskStatus.Todo, TaskStatus.InProgress) => true,
            (TaskStatus.InProgress, TaskStatus.Completed) => true,
            (TaskStatus.Todo, TaskStatus.Cancelled) => true,
            (TaskStatus.InProgress, TaskStatus.Cancelled) => true,
            _ => false
        };
}