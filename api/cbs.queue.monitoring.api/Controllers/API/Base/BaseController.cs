using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using cbs.common.infrastructure.BrokenRules;
using cbs.common.infrastructure.Domain;
using cbs.common.infrastructure.Extensions;
using cbs.common.infrastructure.Paging;
using cbs.common.infrastructure.ResourceParameters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace cbs.queue.monitoring.api.Controllers.API.Base;

/// <summary>
/// BaseController
/// </summary>
public abstract class BaseController : ControllerBase
{
    private IActionResult BadRequestSerialized(BusinessResult businessResult) =>
        BadRequest(new ErrorResponse(businessResult.BrokenRules.Select(x => x.Rule)));

    /// <summary>
    /// OkOrBadRequest
    /// </summary>
    /// <param name="businessResult"></param>
    /// <returns></returns>
    protected IActionResult OkOrBadRequest(BusinessResult businessResult)
    {
        return !businessResult.IsSuccess() ? BadRequestSerialized(businessResult) : Ok();
    }
    /// <summary>
    /// OkOrNoResult
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected IActionResult OkOrNoResult(object input)
    {
        if (!input.IsNull())
            return Ok(input);

        return NoContent();
    }

    /// <summary>
    /// CreatedOrNoResult
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected IActionResult CreatedOrNoResult(object input)
    {
        if (!input.IsNull())
            return Created(nameof(input), input);

        return NoContent();
    }


    /// <summary>
    /// CreateLinksFor
    /// </summary>
    /// <param name="urlHelper"></param>
    /// <param name="routeName"></param>
    /// <param name="id"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    protected IEnumerable<LinkDto> CreateLinksFor(IUrlHelper urlHelper, string routeName, Guid id, string fields)
    {
        var links = new List<LinkDto>();

        links.Add(string.IsNullOrWhiteSpace(fields)
            ? Url.LinkDto(urlHelper.Link(routeName, new { id = id }), "self", "GET")
            : Url.LinkDto(urlHelper.Link(routeName, new { id = id, fields = fields }), "self", "GET"));

        return links;
    }

    /// <summary>
    /// CreateLinksForList
    /// </summary>
    /// <param name="urlHelper"></param>
    /// <param name="routeName"></param>
    /// <param name="parameters"></param>
    /// <param name="hasNext"></param>
    /// <param name="hasPrevious"></param>
    /// <returns></returns>
    protected IEnumerable<LinkDto> CreateLinksForList(IUrlHelper urlHelper, string routeName, BaseResourceParameters parameters, bool hasNext, bool hasPrevious)
    {
        var links = new List<LinkDto>
        {
            new LinkDto(CreateResourceUri(urlHelper, routeName, parameters, ResourceUriType.Current), "self", "GET")
        };

        if (hasNext)
        {
            links.Add(
                new LinkDto(CreateResourceUri(urlHelper, routeName, parameters, ResourceUriType.NextPage), "nextPage", "GET"));
        }

        if (hasPrevious)
        {
            links.Add(
                new LinkDto(CreateResourceUri(urlHelper, routeName, parameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
        }

        return links;
    }

    /// <summary>
    /// CreateResourceUri
    /// </summary>
    /// <param name="urlHelper"></param>
    /// <param name="routeName"></param>
    /// <param name="parameters"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    protected string CreateResourceUri(IUrlHelper urlHelper, string routeName, BaseResourceParameters parameters, ResourceUriType type)
    {
        switch (type)
        {
            case ResourceUriType.PreviousPage:
                return urlHelper.Link(routeName,
                    new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        searchQuery = parameters.SearchQuery,
                        pageNumber = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize
                    });
            case ResourceUriType.NextPage:
                return urlHelper.Link(routeName,
                    new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        searchQuery = parameters.SearchQuery,
                        pageNumber = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize
                    });
            case ResourceUriType.Current:
            default:
                return urlHelper.Link(routeName,
                    new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        searchQuery = parameters.SearchQuery,
                        pageNumber = parameters.PageIndex,
                        pageSize = parameters.PageSize
                    });
        }
    }

    /// <summary>
    /// CreateOkWithMetaData
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="pagedResponse"></param>
    /// <param name="mediaType"></param>
    /// <param name="parameters"></param>
    /// <param name="urlHelper"></param>
    /// <param name="routeNameForSingle"></param>
    /// <param name="routeNameForList"></param>
    /// <returns></returns>
    protected OkObjectResult CreateOkWithMetaData<TEntity>(PagedList<TEntity> pagedResponse, string mediaType, BaseResourceParameters parameters, IUrlHelper urlHelper, string routeNameForSingle, string routeNameForList)
    {
        var items = pagedResponse as IEnumerable<TEntity>;

        if (mediaType.Contains("application/vnd.marvin.hateoas+json"))
        {
            var paginationMetadata = new
            {
                totalCount = pagedResponse.TotalCount,
                pageSize = pagedResponse.PageSize,
                currentPage = pagedResponse.CurrentPage,
                totalPages = pagedResponse.TotalPages,
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForList(urlHelper, routeNameForList, parameters, pagedResponse.HasNext, pagedResponse.HasPrevious);

            var shapedResponse = items.ShapeData(parameters.Fields);

            var shapedResponseWithLinks = shapedResponse.Select(resp =>
            {
                var respAsDictionary = resp as IDictionary<string, object>;
                var respLinks = CreateLinksFor(urlHelper, routeNameForSingle, (Guid)respAsDictionary["Id"], parameters.Fields);

                respAsDictionary.Add("links", respLinks);

                return respAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedResponseWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);
        }
        else
        {
            var previousPageLink = pagedResponse.HasPrevious
                ? CreateResourceUri(urlHelper, routeNameForList, parameters, ResourceUriType.PreviousPage)
                : null;

            var nextPageLink = pagedResponse.HasNext
                ? CreateResourceUri(urlHelper, routeNameForList, parameters, ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink,
                totalCount = pagedResponse.TotalCount,
                pageSize = pagedResponse.PageSize,
                currentPage = pagedResponse.CurrentPage,
                totalPages = pagedResponse.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(items.ShapeData(parameters.Fields));
        }
    }
}