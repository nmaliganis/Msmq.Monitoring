using System.Threading;
using System.Threading.Tasks;
using cbs.common.dtos.Cqrs.Messages;
using cbs.common.dtos.DTOs.Messages;
using cbs.common.infrastructure.BrokenRules;
using cbs.queue.monitoring.contracts.V1.Messages;
using MediatR;

namespace cbs.queue.monitoring.services.V1.Messages;

public class DeleteMessageProcessor : IDeleteMessageProcessor, IRequestHandler<DeleteMessageCommand,
    BusinessResult<MessageDto>>
{
    public Task<BusinessResult<MessageDto>> DeleteMessageAsync(DeleteMessageCommand deleteCommand)
    {
        throw new System.NotImplementedException();
    }

    public Task<BusinessResult<MessageDto>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
} //Class : DeleteHardMessageProcessor