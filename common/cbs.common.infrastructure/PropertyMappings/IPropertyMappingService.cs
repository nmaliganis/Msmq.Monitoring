﻿using System.Collections.Generic;

namespace cbs.common.infrastructure.PropertyMappings;

public interface IPropertyMappingService
{
    bool ValidMappingExistsFor<TSource, TDestination>(string fields);

    Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
}