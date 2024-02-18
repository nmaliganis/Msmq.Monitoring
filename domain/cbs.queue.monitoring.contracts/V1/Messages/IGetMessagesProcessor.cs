using System.Threading.Tasks;
using cbs.common.dtos.Cqrs.Messages;
using cbs.common.dtos.DTOs.Messages;
using cbs.common.infrastructure.BrokenRules;
using cbs.common.infrastructure.Paging;

namespace cbs.queue.monitoring.contracts.V1.Messages;

public interface IGetMessagesProcessor
{
    Task<BusinessResult<PagedList<MessageDto>>> GetMessagesAsync(GetMessagesQuery qry);
}