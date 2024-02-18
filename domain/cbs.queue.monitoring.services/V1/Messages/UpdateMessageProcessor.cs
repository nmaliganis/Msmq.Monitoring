using System;
using System.Linq;
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
using cbs.queue.monitoring.model.Messages;
using MediatR;
using Serilog;

namespace cbs.queue.monitoring.services.V1.Messages;

public class UpdateMessageProcessor : IUpdateMessageProcessor, IRequestHandler<UpdateMessageCommand, BusinessResult<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IAutoMapper _autoMapper;

    public UpdateMessageProcessor(IMessageRepository messageRepository, IAutoMapper autoMapper)
    {
        this._messageRepository = messageRepository;
        this._autoMapper = autoMapper;
    }
    public async Task<BusinessResult<MessageDto>> UpdateMessageAsync(UpdateMessageCommand cmd)
    {
        var bc = new BusinessResult<MessageDto>(new MessageDto());

        if (cmd.IsNull())
        {
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError("ERROR_UPDATE_COMMAND_MODEL_Message"));
            return await Task.FromResult(bc);
        }

        try
        {
            var messageToBeModified = await _messageRepository.FindOneMessageById(cmd.MessageIdToBeModified);

            if (messageToBeModified.IsNull())
            {
                bc.AddBrokenRule(BusinessError.CreateInstance(nameof(cmd.MessageIdToBeModified), "ERROR_UPDATE_COMMAND_Message_NOT_EXIST"));
                return bc;
            }
            
            messageToBeModified.InjectWithValues(
                cmd.MessageForModificationParameters.Gender, 
                cmd.MessageForModificationParameters.Firstname, 
                cmd.MessageForModificationParameters.Lastname, 
                cmd.MessageForModificationParameters.Dob, 
                cmd.MessageForModificationParameters.Calls, 
                cmd.MessageForModificationParameters.Title,
                cmd.MessageForModificationParameters.Street,
                cmd.MessageForModificationParameters.City,
                cmd.MessageForModificationParameters.Postcode,
                cmd.MessageForModificationParameters.Country
                );
            
            this.ThrowExcIfMessageCannotBeCreated(messageToBeModified);
            await this.ThrowExcIfThisMessageAlreadyExist(messageToBeModified);
            
            Log.Information(
                $"Update Message: {cmd.MessageForModificationParameters.Firstname} {cmd.MessageForModificationParameters.Lastname}" +
                "--UpdateMessageAsync--  @NotComplete@ [UpdateMessageProcessor]. " +
                "Message: Just Before MakeItPersistence");

            MakeMessagePersistent(messageToBeModified);

            Log.Information(
                $"Update Message: {cmd.MessageForModificationParameters.Firstname} {cmd.MessageForModificationParameters.Lastname}" +
                "--UpdateMessageAsync--  @NotComplete@ [UpdateMessageProcessor]. " +
                "Message: Just After MakeItPersistence");

            bc.Model = this._autoMapper.Map<MessageDto>(messageToBeModified);
        }       
        catch (Exception exxx)
        {
            string errorMessage = "ERROR_CREATE_COMMAND_Message";
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError(errorMessage));
            Log.Error(
                $"Update Message: {cmd.MessageForModificationParameters.Firstname} {cmd.MessageForModificationParameters.Lastname}" +
                $"Error Message:{errorMessage}" +
                $"--UpdateMessageAsync--  @fail@ [UpdateMessageProcessor]. " +
                $"@innerfault:{exxx.Message} and {exxx.InnerException}");
        }

        return await Task.FromResult(bc);
    }

    public async Task<BusinessResult<MessageDto>> Handle(UpdateMessageCommand cmd, CancellationToken cancellationToken)
    {
        return await UpdateMessageAsync(cmd);
    }
    
    private void MakeMessagePersistent(Message messageToBeModified)
    {
        _messageRepository.UpdateMessage(messageToBeModified.Id, messageToBeModified);
    }
    
    private async Task ThrowExcIfThisMessageAlreadyExist(Message messageToBeCreated)
    {
        var messageRetrieved = await this._messageRepository.FindOneMessageByFirstnameAndLastname(messageToBeCreated.Firstname, messageToBeCreated.Lastname);
        if (!messageRetrieved.IsNull())
        {
            throw new MessageAlreadyExistsException($"{messageToBeCreated.Firstname} {messageToBeCreated.Lastname}",
                messageToBeCreated.GetBrokenRulesAsString());
        }
    }

    private void ThrowExcIfMessageCannotBeCreated(Message messageToBeCreated)
    {
        bool canBeCreated = !messageToBeCreated.GetBrokenRules().Any();
        if (!canBeCreated)
        {
            throw new InvalidMessageException(messageToBeCreated.GetBrokenRulesAsString());
        }
    }
}