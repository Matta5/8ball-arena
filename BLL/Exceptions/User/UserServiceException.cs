namespace BLL.Exceptions;

public class UserServiceException : Exception
{
    public UserServiceException(string message) : base(message)
    {
    }

    public UserServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
