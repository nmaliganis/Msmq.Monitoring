using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cbs.common.dtos.Cqrs.Messages;
using cbs.common.dtos.DTOs.Messages;
using cbs.common.infrastructure.BrokenRules;
using cbs.common.infrastructure.Extensions;
using cbs.common.infrastructure.Paging;
using cbs.common.infrastructure.PropertyMappings;
using cbs.common.infrastructure.Types;
using cbs.queue.monitoring.contracts.ContractRepositories;
using cbs.queue.monitoring.contracts.V1.Messages;
using cbs.queue.monitoring.model.Messages;
using MediatR;

namespace cbs.queue.monitoring.services.V1.Messages;

public class GetMessagesProcessor : IGetMessagesProcessor, IRequestHandler<GetMessagesQuery, BusinessResult<PagedList<MessageDto>>>
{
    private readonly IAutoMapper _autoMapper;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IMessageRepository _messageRepository;

    public GetMessagesProcessor(IMessageRepository messageRepository,
        IAutoMapper autoMapper, IPropertyMappingService propertyMappingService)
    {
        _messageRepository = messageRepository;
        _autoMapper = autoMapper;
        _propertyMappingService = propertyMappingService;
    }

    public async Task<BusinessResult<PagedList<MessageDto>>> Handle(GetMessagesQuery qry, CancellationToken cancellationToken)
    {
        return await GetMessagesAsync(qry);
    }

    public async Task<BusinessResult<PagedList<MessageDto>>> GetMessagesAsync(GetMessagesQuery qry)
    {
        var collectionBeforePaging =
            await _messageRepository.FindMessagesPagedOf(qry.PageIndex, qry.PageSize);
            
        collectionBeforePaging.ApplySort(qry.OrderBy + " " + qry.SortDirection,
            _propertyMappingService.GetPropertyMapping<MessageDto, Message>());

        if (!string.IsNullOrEmpty(qry.Filter) && !string.IsNullOrEmpty(qry.SearchQuery))
        {
            var searchQueryForWhereClauseFilterFields = qry.Filter
                .Trim().ToLowerInvariant();

            var searchQueryForWhereClauseFilterSearchQuery = qry.SearchQuery
                .Trim().ToLowerInvariant();

            collectionBeforePaging.QueriedItems = (IQueryable<Message>)collectionBeforePaging
                .QueriedItems
                .AsEnumerable()
                .FilterData(searchQueryForWhereClauseFilterFields, searchQueryForWhereClauseFilterSearchQuery);
        }

        var afterPaging = PagedList<Message>
            .Create(collectionBeforePaging, qry.PageIndex, qry.PageSize);

        var items = _autoMapper.Map<List<MessageDto>>(afterPaging);

        var result = new PagedList<MessageDto>(
            items,
            afterPaging.TotalCount,
            afterPaging.CurrentPage,
            afterPaging.PageSize);

        var bc = new BusinessResult<PagedList<MessageDto>>(result);

        return await Task.FromResult(bc);
    }
}