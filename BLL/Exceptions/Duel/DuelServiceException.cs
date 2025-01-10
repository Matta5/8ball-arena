namespace BLL.Exceptions.Duel;


public class DuelServiceException : Exception
{
    public DuelServiceException(string message) : base(message)
    {
    }

    public DuelServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
