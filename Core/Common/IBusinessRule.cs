namespace Core.Common;

public interface IBusinessRule
{
    bool IsBroken();

    string Message { get; }
}