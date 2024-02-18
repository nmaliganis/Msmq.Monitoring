using System;
using System.Collections.Generic;
using System.Linq;
using cbs.common.infrastructure.Queries;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace cbs.common.infrastructure.Domain;

public static class UriHelpers
{
    public static LinkDto LinkDto(this IUrlHelper helper, string rel,
        string method, string action)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        return helper.LinkDto(rel, method, action, null, null);
    }

    public static LinkDto LinkDto(this IUrlHelper helper, string rel,
        string method, string action, object values)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        return helper.LinkDto(rel, method, action, null, values);
    }


    public static LinkDto LinkDto(this IUrlHelper helper, string rel,
        string method, string action, string controller)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        return helper.LinkDto(rel, method, action, controller, null);
    }


    public static LinkDto LinkDto(
        this IUrlHelper helper,
        string rel,
        string method,
        string action,
        string controller,
        object values)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        if (string.IsNullOrWhiteSpace(rel)) throw new ArgumentNullException(nameof(rel));

        if (string.IsNullOrWhiteSpace(method)) throw new ArgumentNullException(nameof(method));

        return new LinkDto(
            helper.Action(action, controller, values,
                helper.ActionContext.HttpContext.Request.Scheme),
            rel,
            method);
    }


    public static LinkDto LinkDto<T>(this IUrlHelper helper, string rel,
        string method, string action)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));


        return helper.LinkDto(rel, method, action, null, null);
    }

    public static LinkDto LinkDto<T>(this IUrlHelper helper, string rel,
        string method, string action, object values)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        return helper.LinkDto(rel, method, action, null, values);
    }


    public static LinkDto LinkDto<T>(this IUrlHelper helper, string rel,
        string method, string action, string controller)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        return helper.LinkDto(rel, method, action, controller, null);
    }


    public static LinkDto LinkDto<T>(
        this IUrlHelper helper,
        string rel,
        string method,
        string action,
        string controller,
        object values)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        if (string.IsNullOrWhiteSpace(rel)) throw new ArgumentNullException(nameof(rel));

        if (string.IsNullOrWhiteSpace(method)) throw new ArgumentNullException(nameof(method));

        return new LinkDto(
            helper.Action(action, controller, values,
                helper.ActionContext.HttpContext.Request.Scheme),
            rel,
            method);
    }

    public static string AppendToUrl<T>(this ISelectOptions<T> options, string url)
    {
        if (string.IsNullOrWhiteSpace(options?.Select)) return url;
        var uri = new Uri(url);
        var baseUri =
            uri.GetComponents(UriComponents.Path,
                UriFormat.UriEscaped);

        var query = QueryHelpers.ParseQuery(uri.Query);

        if (!string.IsNullOrWhiteSpace(options?.Select)) query.Add("$select", options?.Select);

        var qb = new QueryBuilder(query
            .SelectMany(x => x.Value,
                (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList());
        return baseUri + qb.ToQueryString();
    }
    public static string AppendToUrl<T>(this IQueryOptions<T> options, string url)
    {
        if (string.IsNullOrWhiteSpace(url) || options == null) return url;
        var uri = new Uri(url);
        var baseUri =
            uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
                UriFormat.UriEscaped);

        var query = QueryHelpers.ParseQuery(uri.Query);
        if (options.Skip.HasValue && options.Skip.Value > 0) query.Add("$skip", options.Skip.Value.ToString());
        if (options.Top.HasValue) query.Add("$top", options.Top.Value.ToString());
        if (!string.IsNullOrWhiteSpace(options.Filter)) query.Add("$filter", options.Filter);
        if (!string.IsNullOrWhiteSpace(options.Sort)) query.Add("$orderby", options.Sort);
        if (!string.IsNullOrWhiteSpace(options.Select)) query.Add("$select", options.Select);


        var qb = new QueryBuilder(query
            .SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList());
        return baseUri + qb.ToQueryString();
    }
    public static LinkDto LinkDto<T>(
        this IUrlHelper helper,
        string rel,
        string method,
        string action,
        string controller,
        object values,
        ISelectOptions<T> filters = null)
    {
        if (helper == null) throw new ArgumentNullException(nameof(helper));

        if (string.IsNullOrWhiteSpace(rel)) throw new ArgumentNullException(nameof(rel));

        if (string.IsNullOrWhiteSpace(method)) throw new ArgumentNullException(nameof(method));

        if (filters != null)
            return new LinkDto(
                filters.AppendToUrl(helper.Action(action, controller, values,
                    helper.ActionContext.HttpContext.Request.Scheme)),
                rel,
                method);

        return new LinkDto(
            helper.Action(action, controller, values,
                helper.ActionContext.HttpContext.Request.Scheme),
            rel,
            method);
    }
}