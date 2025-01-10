namespace BLL.Exceptions.Duel;

public class DuelRepositoryException : Exception
{
    public DuelRepositoryException(string message) : base(message)
    {
    }

    public DuelRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
