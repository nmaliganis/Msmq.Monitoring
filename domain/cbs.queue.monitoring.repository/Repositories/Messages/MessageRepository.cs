using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cbs.common.infrastructure.Exceptions.Messages;
using cbs.common.infrastructure.Paging;
using cbs.common.infrastructure.Queries;
using cbs.queue.monitoring.contracts.ContractRepositories;
using cbs.queue.monitoring.model.Messages;
using JsonFlatFileDataStore;
using Serilog;

namespace cbs.queue.monitoring.repository.Repositories.Messages;

public class MessageRepository : IMessageRepository
{
    public DataStore Store { get; private set; }
    
    private readonly string _folderDetails = string.Empty;
    
    public MessageRepository()
    {
        this._folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $@"wwwroot\Messages.json");  
        try
        {
            Store = new DataStore(this._folderDetails);
        }
        catch (Exception e)
        {
            Log.Error(
                $"Loading Json Error:{e.Message} and {e.InnerException}");
            throw;
        }
    }
    
    public Task<Message> FindOneMessageById(string idMessage)
    {
        var collection = Store.GetCollection<Message>("Messages");

        var fetchedMessages = collection
            .AsQueryable()
            .Where(p => p.Id == idMessage).ToList();
        
        if (fetchedMessages.Count > 1)
            throw new MultipleMessageWhereFoundException($"More than one with same Id:{idMessage}");
        if (fetchedMessages.Count < 0)
            throw new MessageDoesNotExistException($"More than one with same Id:{idMessage}");
        
        return Task.Run(() => fetchedMessages.FirstOrDefault());
    }

    public Task<Message> FindOneMessageByFirstnameAndLastname(string firstname, string lastname)
    {
        var collection = Store.GetCollection<Message>("Messages");

        var fetchedMessages = collection
            .AsQueryable()
            .Where(p => p.Firstname == firstname)
            .Where(p=> p.Lastname == lastname)
            .ToList();
        
        if (fetchedMessages.Count > 1)
            throw new MultipleMessageWhereFoundException($"More than one with same Name:{firstname} {lastname}");
        
        return Task.Run(() => fetchedMessages.FirstOrDefault());
    }

    public async Task CreateMessage(Message newMessage)
    {
        var collection = Store.GetCollection<Message>("Messages");
        try
        {
            await collection.InsertOneAsync(newMessage);
        }
        catch (Exception e)
        {
            Log.Error(
                $"Insertion Error:{e.Message} and {e.InnerException}");
            throw;
        }
    }

    public async Task UpdateMessage(string idMessage, Message modifiedMessage)
    {
        var collection = Store.GetCollection<Message>("Messages");
        try
        {
            await collection.UpdateOneAsync(s=>s.Id == idMessage, modifiedMessage);
        }
        catch (Exception e)
        {
            Log.Error(
                $"Insertion Error:{e.Message} and {e.InnerException}");
            throw;
        }
    }

    public async Task DeleteMessage(string idMessage)
    {
        var collection = Store.GetCollection<Message>("Messages");
        try
        {
            await collection.DeleteOneAsync(s=>s.Id == idMessage);
        }
        catch (Exception e)
        {
            Log.Error(
                $"Deletion Error:{e.Message} and {e.InnerException}");
            throw;
        }
    }

    public async Task<QueryResult<Message>> FindMessagesPagedOf(int? pageNum, int? pageSize)
    {
        var collection = Store.GetCollection<Message>("Messages");
        
        if (pageNum == -1 & pageSize == -1)
        {
            return new QueryResult<Message>(collection.AsQueryable(),
                    collection.Count,
                    (int) pageSize)
                ;
        }

        return new QueryResult<Message>(collection.AsQueryable()
                    .Skip(ResultsPagingUtility.CalculateStartIndex((int) pageNum, (int) pageSize))
                    .Take((int) pageSize).AsQueryable(),
                collection.Count,
                (int) pageSize)
            ;
    }

    public Task<int> FindMessagesCount()
    {
        throw new NotImplementedException();
    }
}