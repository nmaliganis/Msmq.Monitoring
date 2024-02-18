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

public class CreateMessageProcessor : ICreateMessageProcessor, IRequestHandler<CreateMessageCommand, BusinessResult<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IAutoMapper _autoMapper;

    public CreateMessageProcessor(IMessageRepository messageRepository, IAutoMapper autoMapper)
    {
        this._messageRepository = messageRepository;
        this._autoMapper = autoMapper;
    }

    public async Task<BusinessResult<MessageDto>> CreateMessageAsync(CreateMessageCommand cmd)
    {
        var bc = new BusinessResult<MessageDto>(new MessageDto());

        if (cmd.IsNull())
        {
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError("ERROR_CREATE_COMMAND_MODEL_Message"));
            return await Task.FromResult(bc);
        }

        try
        {
            var messageToBeCreated = new Message();

            messageToBeCreated.InjectWithValues(
                cmd.MessageForCreationParameters.Gender, 
                cmd.MessageForCreationParameters.Firstname, 
                cmd.MessageForCreationParameters.Lastname, 
                cmd.MessageForCreationParameters.Dob, 
                cmd.MessageForCreationParameters.Calls, 
                cmd.MessageForCreationParameters.Title,
                cmd.MessageForCreationParameters.Street,
                cmd.MessageForCreationParameters.City,
                cmd.MessageForCreationParameters.Postcode,
                cmd.MessageForCreationParameters.Country
                );
            
            this.ThrowExcIfMessageCannotBeCreated(messageToBeCreated);
            await this.ThrowExcIfThisMessageAlreadyExist(messageToBeCreated);
            
            Log.Information(
                $"Create Message: {cmd.MessageForCreationParameters.Firstname} {cmd.MessageForCreationParameters.Lastname}" +
                "--CreateMessageAsync--  @NotComplete@ [CreateMessageProcessor]. " +
                "Message: Just Before MakeItPersistence");

            MakeMessagePersistent(messageToBeCreated);

            Log.Information(
                $"Create Message: {cmd.MessageForCreationParameters.Firstname} {cmd.MessageForCreationParameters.Lastname}" +
                "--CreateMessageAsync--  @NotComplete@ [CreateMessageProcessor]. " +
                "Message: Just After MakeItPersistence");

            bc.Model = this._autoMapper.Map<MessageDto>(messageToBeCreated);
        }       
        catch (Exception exxx)
        {
            string errorMessage = "ERROR_CREATE_COMMAND_Message";
            bc.Model = null;
            bc.AddBrokenRule(new BusinessError(errorMessage));
            Log.Error(
                $"Create Message: {cmd.MessageForCreationParameters.Firstname} {cmd.MessageForCreationParameters.Lastname}" +
                $"Error Message:{errorMessage}" +
                $"--CreateMessageAsync--  @fail@ [CreateMessageProcessor]. " +
                $"@innerfault:{exxx.Message} and {exxx.InnerException}");
        }

        return await Task.FromResult(bc);
    }

    private void MakeMessagePersistent(Message MessageToBeCreated)
    {
        _messageRepository.CreateMessage(MessageToBeCreated);
    }
    
    private async Task ThrowExcIfThisMessageAlreadyExist(Message MessageToBeCreated)
    {
        var MessageRetrieved = await this._messageRepository.FindOneMessageByFirstnameAndLastname(MessageToBeCreated.Firstname, MessageToBeCreated.Lastname);
        if (!MessageRetrieved.IsNull())
        {
            throw new MessageAlreadyExistsException($"{MessageToBeCreated.Firstname} {MessageToBeCreated.Lastname}",
                MessageToBeCreated.GetBrokenRulesAsString());
        }
    }

    private void ThrowExcIfMessageCannotBeCreated(Message MessageToBeCreated)
    {
        bool canBeCreated = !MessageToBeCreated.GetBrokenRules().Any();
        if (!canBeCreated)
        {
            throw new InvalidMessageException(MessageToBeCreated.GetBrokenRulesAsString());
        }
    }

    public async Task<BusinessResult<MessageDto>> Handle(CreateMessageCommand cmd, CancellationToken cancellationToken)
    {
        return await CreateMessageAsync(cmd);
    }
}//Class : CreateMessageProcessor