using System.Collections.Generic;
using System.Linq;

namespace cbs.common.infrastructure.Extensions;

public class ErrorResponse
{
    public ErrorResponse() { }

    public ErrorResponse(IEnumerable<string> messages) => Messages = messages.ToArray();

    public string[] Messages { get; set; }
}