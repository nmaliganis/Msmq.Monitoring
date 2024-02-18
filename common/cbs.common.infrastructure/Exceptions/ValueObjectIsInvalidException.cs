using System;

namespace cbs.common.infrastructure.Exceptions;

public class ValueObjectIsInvalidException : Exception
{
    public ValueObjectIsInvalidException(string message)
        : base(message)
    {
    }
}