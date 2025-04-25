using System.Runtime.Serialization;

namespace VotingContract;

[GenerateSerializer]
public class ThrottlingException : Exception
{
    [Id(0)]
    private readonly string _message;

    public ThrottlingException() { }
    
    public ThrottlingException(string message) : base(message) 
    { 
        _message = message;
    }
    
    public ThrottlingException(string message, Exception innerException) : base(message, innerException) 
    { 
        _message = message;
    }
}