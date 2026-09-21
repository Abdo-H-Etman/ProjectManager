using TaskStatus = Domain.Enums.TaskStatus;

namespace Domain.Exceptions;

public class InvalidTaskStatusTransitionException : Exception
{
    public InvalidTaskStatusTransitionException(TaskStatus currentStatus, TaskStatus newStatus)
        : base($"Task status cannot transition from {currentStatus} to {newStatus}.")
    {
    }
}