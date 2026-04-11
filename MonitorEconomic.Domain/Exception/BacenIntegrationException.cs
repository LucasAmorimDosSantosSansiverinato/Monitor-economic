namespace MonitorEconomic.Domain.Exceptions;

public class BacenIntegrationException : Exception
{
    public BacenIntegrationException(string message) : base(message)
    {
    }

    public BacenIntegrationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}