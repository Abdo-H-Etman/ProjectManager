namespace Domain.Exceptions;

public class DeletedException : Exception
{
    public DeletedException(string name, object key)
        : base($"Entity \"{name}\" ({key}) has been deleted and can no longer be used.")
    {
    }
}