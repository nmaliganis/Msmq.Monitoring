using System;

namespace cbs.common.infrastructure.BrokenRules;

public sealed class BusinessError
{
    public static BusinessError CreateInstance(string property, string rule)
    {
        return new BusinessError(property, rule);
    }

    private BusinessError(string property, string rule)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));
        Rule = rule ?? throw new ArgumentNullException(nameof(rule));
    }

    public BusinessError(string rule) => Rule = rule ?? throw new ArgumentNullException(nameof(rule));

    public string Property { get; }

    public string Rule { get; }

    public override string ToString()
    {
        return string.Format($"{Rule}-{Property}");
    }
}