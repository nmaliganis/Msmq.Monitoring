using System;
using System.Net;
using System.Threading.Tasks;
using cbs.common.dtos.Cqrs.Messages;
using cbs.common.dtos.DTOs.Messages;
using cbs.common.dtos.ResourceParameters.Messages;
using cbs.common.infrastructure.BrokenRules;
using cbs.common.infrastructure.Extensions;
using cbs.common.infrastructure.Paging;
using cbs.common.infrastructure.PropertyMappings;
using cbs.common.infrastructure.TypeHelpers;
using cbs.queue.monitoring.api.Controllers.API.Base;
using cbs.queue.monitoring.api.Validators;
using cbs.queue.monitoring.model.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace cbs.queue.monitoring.api.Controllers.API.V1;

/// <summary>
/// Message Controller
/// </summary>
[Produces("application/json")]
[ApiVersion("1.0")]
[ResponseCache(Duration = 0, NoStore = true, VaryByHeader = "*")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MessagesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IUrlHelper _urlHelper;
    private readonly ITypeHelperService _typeHelperService;
    private readonly IPropertyMappingService _propertyMappingService;


    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="urlHelper"></param>
    /// <param name="typeHelperService"></param>
    /// <param name="propertyMappingService"></param>
    public MessagesController(
        IMediator mediator,
        IUrlHelper urlHelper,
        ITypeHelperService typeHelperService,
        IPropertyMappingService propertyMappingService)
    {
        this._mediator = mediator;
        this._urlHelper = urlHelper;
        this._typeHelperService = typeHelperService;
        this._propertyMappingService = propertyMappingService;
    }

    /// <summary>
    /// Get - Fetch all Messages
    /// </summary>
    /// <param name="parameters">Message parameters for fetching</param>
    /// <param name="mediaType"></param>
    /// <remarks>Fetch all Messages </remarks>
    /// <response code="200">Resource retrieved correctly</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error.</response>
    [HttpGet(Name = "FetchMessagesRoot")]
    [ValidateModel]
    [ProducesResponseType(typeof(BusinessResult<PagedList<MessageDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FetchMessagesAsync(
        [FromQuery] GetMessagesResourceParameters parameters,
        [FromHeader(Name = "Accept")] string mediaType)
    {
        if (!string.IsNullOrEmpty(parameters.OrderBy)
            && !this._propertyMappingService.ValidMappingExistsFor<MessageDto, Message>(parameters.OrderBy))
        {
            return this.BadRequest("ERROR_RESOURCE_PARAMETER");
        }

        if (!this._typeHelperService.TypeHasProperties<MessageDto>
                (parameters.Fields))
        {
            return this.BadRequest("ERROR_FIELDS_PARAMETER");
        }

        var fetchedItineraries = await this._mediator.Send(new GetMessagesQuery(parameters));

        if (fetchedItineraries.IsNull())
        {
            return this.NotFound("ERROR_FETCH_MessageS");
        }

        if (!fetchedItineraries.IsSuccess())
        {
            return this.OkOrBadRequest(fetchedItineraries.BrokenRules);
        }

        if (fetchedItineraries.Status == BusinessResultStatus.Fail)
        {
            return this.OkOrNoResult(fetchedItineraries.BrokenRules);
        }

        var responseWithMetaData = this.CreateOkWithMetaData(fetchedItineraries.Model, mediaType,
            parameters, this._urlHelper, "GetMessageByIdAsync", "FetchMessagesAsync");
        return this.Ok(responseWithMetaData);
    }

    /// <summary>
    /// Get - Fetch one Message
    /// </summary>
    /// <param name="id">Message Id for fetching</param>
    /// <remarks>Fetch one Message </remarks>
    /// <response code="200">Resource retrieved correctly</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error.</response>
    [HttpGet("{id}", Name = "GetMessageByIdAsync")]
    [ValidateModel]
    [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetMessageByIdAsync(string id)
    {
        var fetchedMessage = await this._mediator.Send(new GetMessageByIdQuery(id));

        if (fetchedMessage.IsNull())
        {
            return this.NotFound("ERROR_FETCH_Message");
        }

        if (!fetchedMessage.IsSuccess())
        {
            return this.OkOrBadRequest(fetchedMessage.BrokenRules);
        }

        if (fetchedMessage.Status == BusinessResultStatus.Fail)
        {
            return this.OkOrNoResult(fetchedMessage.BrokenRules);
        }

        return this.Ok(fetchedMessage);
    }

    /// <summary>
    /// Post - Create a Message
    /// </summary>
    /// <param name="MessageForCreationParameters">CreateMessageResourceParameters for creation</param>
    /// <remarks>Create Message </remarks>
    /// <response code="201">Resource Creation Finished</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error.</response>
    [HttpPost(Name = "CreateMessageRoot")]
    [ValidateModel]
    [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateMessageAsync([FromBody] CreateMessageResourceParameters MessageForCreationParameters)
    {
        var createdMessage = await this._mediator.Send(new CreateMessageCommand(MessageForCreationParameters));
        
        if (!createdMessage.IsSuccess())
        {
            return this.OkOrBadRequest(createdMessage.BrokenRules);
        }

        if (createdMessage.Status == BusinessResultStatus.Fail)
        {
            Log.Information(
                $"--Method:PostMessageRouteAsync -- Message:Message_CREATION_SUCCESSFULLY" +
                $" -- Datetime:{DateTime.Now} -- MessageInfo:{createdMessage.Model.Id}");
            return this.OkOrNoResult(createdMessage.BrokenRules);
        }
        return this.CreatedOrNoResult(createdMessage);
    }

    /// <summary>
    /// Put - Update a Message
    /// </summary>
    /// <param name="id">Message Id for modification</param>
    /// <param name="request">UpdateMessageResourceParameters for modification</param>
    /// <remarks>Update Message </remarks>
    /// <response code="200">Resource Modification Finished</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error.</response>
    [HttpPut("{id}", Name = "UpdateMessageAsync")]
    [ValidateModel]
    [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateMessageAsync(string id, [FromBody] UpdateMessageResourceParameters request)
    {
        var response = await this._mediator.Send(new UpdateMessageCommand(id, request));

        if (response.IsNull())
        {
            return this.NotFound("Message_NOT_FOUND");
        }

        if (!response.IsSuccess())
        {
            return this.OkOrBadRequest(response.BrokenRules);
        }

        if (response.Status == BusinessResultStatus.Fail)
        {
            return this.OkOrNoResult(response.BrokenRules);
        }

        return this.Ok(response);
    }
    

    /// <summary>
    /// Delete - Remove an existing Message
    /// </summary>
    /// <param name="id">Message Id for deletion</param>
    /// <remarks>Delete existing Message </remarks>
    /// <response code="200">Resource Deletion Finished</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error.</response>
    [HttpDelete("{id}", Name = "DeleteMessageAsync")]
    [ValidateModel]
    [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteMessageAsync(string id)
    {
        var response = await this._mediator.Send(new DeleteMessageCommand(id));

        if (response.IsNull())
        {
            return this.NotFound("ERROR_DELETE_Message");
        }

        if (!response.IsSuccess())
        {
            return this.OkOrBadRequest(response.BrokenRules);
        }

        if (response.Status == BusinessResultStatus.Fail)
        {
            return this.OkOrNoResult(response.BrokenRules);
        }

        return this.Ok(response);
    }
}//Class : MessagesController