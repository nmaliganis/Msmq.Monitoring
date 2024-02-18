using System;

namespace cbs.common.infrastructure.Exceptions.Messages
{
    public class MultipleMessageWhereFoundException : Exception
    {
        public string Detail { get; }

        public MultipleMessageWhereFoundException(string detail)
        {
            this.Detail = detail;
        }

        public override string Message => $" Multiple Message with :{Detail} found.";
    }
}