using System.Threading.Tasks;
using cbs.common.dtos.Cqrs.Messages;
using cbs.common.dtos.DTOs.Messages;
using cbs.common.infrastructure.BrokenRules;

namespace cbs.queue.monitoring.contracts.V1.Messages;

public interface IUpdateMessageProcessor
{
    Task<BusinessResult<MessageDto>> UpdateMessageAsync(UpdateMessageCommand updateCommand);
}