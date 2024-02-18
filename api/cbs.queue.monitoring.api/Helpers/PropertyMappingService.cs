using System;
using System.Collections.Generic;
using cbs.common.dtos.DTOs.Messages;
using cbs.common.infrastructure.PropertyMappings;
using cbs.queue.monitoring.model.Messages;

namespace cbs.queue.monitoring.api.Helpers;

/// <summary>
/// Class
/// </summary>
public class PropertyMappingService : BasePropertyMapping
{
    private readonly Dictionary<string, PropertyMappingValue> _MessagePropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
            { "Gender", new PropertyMappingValue(new List<string>() { "Gender" }) },
            { "Firstname", new PropertyMappingValue(new List<string>() { "Firstname" }) },
            { "Lastname", new PropertyMappingValue(new List<string>() { "Lastname" }) },
            { "Title", new PropertyMappingValue(new List<string>() { "Title" }) },
            { "Calls", new PropertyMappingValue(new List<string>() { "Calls" }) },
            { "Dob", new PropertyMappingValue(new List<string>() { "Dob" }) },
            { "Nationality", new PropertyMappingValue(new List<string>() { "Nationality" }) },
        };

    private static readonly IList<IPropertyMapping> PropertyMappings = new List<IPropertyMapping>();

    /// <summary>
    /// Ctor
    /// </summary>
    public PropertyMappingService() : base(PropertyMappings)
    {
        PropertyMappings.Add(new PropertyMapping<MessageDto, Message>(_MessagePropertyMapping));
    }
}