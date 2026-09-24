namespace Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "You are not authorized to access this resource.")
        : base(message) { }
}
