namespace Wilczura.Demo.Common.Exceptions;

public class CommonException(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
}