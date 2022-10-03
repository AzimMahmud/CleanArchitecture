namespace Application.Configuration.Behavior.Validation;

public class InvalidCommandException : Exception
{
    public string Details { get; }
    public InvalidCommandException(string message, string details) : base(message)
    {
        this.Details = details;
    }
}