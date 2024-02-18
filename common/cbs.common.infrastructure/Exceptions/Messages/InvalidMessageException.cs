using System;

namespace cbs.common.infrastructure.Exceptions.Messages;

public class InvalidMessageException : Exception
{
    public string BrokenRules { get; private set; }

    public InvalidMessageException(string brokenRules)
    {
        BrokenRules = brokenRules;
    }
}