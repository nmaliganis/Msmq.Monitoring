using System;

namespace cbs.common.infrastructure.Exceptions.Messages;

public class MessageDoesNotExistException : Exception
{
    public string MessageId { get; }

    public MessageDoesNotExistException(string messageId)
    {
        this.MessageId = messageId;
    }

    public override string Message => $"Message with Id: {MessageId}  doesn't exists!";
}