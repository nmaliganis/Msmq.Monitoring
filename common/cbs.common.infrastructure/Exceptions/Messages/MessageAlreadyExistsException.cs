using System;

namespace cbs.common.infrastructure.Exceptions.Messages;

public class MessageAlreadyExistsException : Exception
{
    public string Name { get; }
    public string BrokenRules { get; }

    public MessageAlreadyExistsException(string name, string brokenRules)
    {
        Name = name;
        BrokenRules = brokenRules;
    }

    public override string Message => $" Message with Name:{Name} already Exists!\n Additional info:{BrokenRules}";
}