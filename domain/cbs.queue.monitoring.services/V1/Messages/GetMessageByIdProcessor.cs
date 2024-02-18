using System;
using System.Threading;
using System.Threading.Tasks;
using cbs.common.dtos.Cqrs.Messages;
using cbs.common.dtos.DTOs.Messages;
using cbs.common.infrastructure.BrokenRules;
using cbs.common.infrastructure.Exceptions.Messages;
using cbs.common.infrastructure.Extensions;
using cbs.common.infrastructure.Types;
using cbs.queue.monitoring.contracts.ContractRepositories;
using cbs.queue.monitoring.contracts.V1.Messages;
using MediatR;
using Serilog;

namespace cbs.queue.monitoring.services.V1.Messages;

public class GetMessageByIdProcessor : IGetMessageByIdProcessor, IRequestHandler<GetMessageByIdQuery, BusinessResult<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IAutoMapper _autoMapper;
    
    public GetMessageByIdProcessor(IAutoMapper autoMapper, IMessageRepository messageRepository)
    {
        this._autoMapper = autoMapper;
        this._messageRepository = messageRepository;
    }
    
    public async Task<BusinessResult<MessageDto>> GetMessageByIdAsync(string id)
    {
        var bc = new BusinessResult<MessageDto>(new MessageDto());
        
        if (string.IsNullOrEmpty(id))
        {
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError("ERROR_FETCH_COMMAND_Message_ID"));
            return await Task.FromResult(bc);
        }

        try
        {
            var fetchedMessage = await _messageRepository.FindOneMessageById(id);
            
            if(!fetchedMessage.IsNull())
                bc.Model = _autoMapper.Map<MessageDto>(fetchedMessage);
        }
        catch (MessageDoesNotExistException e)
        {
            string errorMessage = "ERROR_FETCH_COMMAND_Message_NOT_EXIST";
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError(errorMessage));
            Log.Error(
                $"Fetch Message: {id}" +
                $"Error Message:{errorMessage}" +
                $"--GetMessageByIdAsync--  @fail@ [GetMessageByIdProcessor]. " +
                $"@innerfault:{e.Message} and {e.InnerException}");
        }
        catch (MultipleMessageWhereFoundException ex)
        {
            string errorMessage = "ERROR_FETCH_COMMAND_Message_MULTIPLE";
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError(errorMessage));
            Log.Error(
                $"Fetch Message: {id}" +
                $"Error Message:{errorMessage}" +
                $"--GetMessageByIdAsync--  @fail@ [GetMessageByIdProcessor]. " +
                $"@innerfault:{ex.Message} and {ex.InnerException}");
        }
        catch (Exception exx)
        {
            string errorMessage = "OTHER_ERROR";
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError(errorMessage));
            Log.Error(
                $"Fetch Message: {id}" +
                $"Error Message:{errorMessage}" +
                $"--GetMessageByIdAsync--  @fail@ [GetMessageByIdProcessor]. " +
                $"@innerfault:{exx.Message} and {exx.InnerException}");
        }

        return await Task.FromResult(bc);
    }

    public async Task<BusinessResult<MessageDto>> Handle(GetMessageByIdQuery qry, CancellationToken cancellationToken)
    {
        return await GetMessageByIdAsync(qry.Id);
    }
}//Class : GetMessageByIdProcessor