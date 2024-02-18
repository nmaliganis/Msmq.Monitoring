using System.Threading.Tasks;
using cbs.common.infrastructure.Queries;
using cbs.queue.monitoring.model.Messages;

namespace cbs.queue.monitoring.contracts.ContractRepositories;

public interface IMessageRepository
{
    Task<Message> FindOneMessageById(string idMessage);
    Task<Message> FindOneMessageByFirstnameAndLastname(string firstname, string lastname);

    Task CreateMessage(Message newMessage);
    Task UpdateMessage(string idMessage, Message modifiedMessage);
    Task DeleteMessage(string idMessage);

    Task<QueryResult<Message>> FindMessagesPagedOf(int? pageNum, int? pageSize);
    Task<int> FindMessagesCount();
}