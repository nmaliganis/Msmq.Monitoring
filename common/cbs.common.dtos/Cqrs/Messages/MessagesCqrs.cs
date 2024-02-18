using cbs.common.dtos.DTOs.Messages;
using cbs.common.dtos.ResourceParameters.Messages;
using cbs.common.infrastructure.BrokenRules;
using cbs.common.infrastructure.Paging;
using MediatR;

namespace cbs.common.dtos.Cqrs.Messages;

// Queries
public record GetMessageByIdQuery(string Id) : IRequest<BusinessResult<MessageDto>>;

public class GetMessagesQuery : GetMessagesResourceParameters, IRequest<BusinessResult<PagedList<MessageDto>>> {
    public GetMessagesQuery(GetMessagesResourceParameters parameters) : base() {
        this.Filter = parameters.Filter;
        this.SearchQuery = parameters.SearchQuery;
        this.Fields = parameters.Fields;
        this.OrderBy = parameters.OrderBy;
        this.SortDirection = parameters.SortDirection;
        this.PageSize = parameters.PageSize;
        this.PageIndex = parameters.PageIndex;
    }
}

// Commands
public record CreateMessageCommand(CreateMessageResourceParameters MessageForCreationParameters)
    : IRequest<BusinessResult<MessageDto>>;

public record UpdateMessageCommand(string MessageIdToBeModified, UpdateMessageResourceParameters MessageForModificationParameters)
    : IRequest<BusinessResult<MessageDto>>;

public record DeleteMessageCommand(string MessageIdToBeDeleted)
    : IRequest<BusinessResult<MessageDto>>;