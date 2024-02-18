namespace cbs.common.infrastructure;

public class BusinessRule
{
    public string Property { get; set; }
    public string Rule { get; set; }

    public BusinessRule()
        : this("Nothing", "Nothing")
    {
    }

    public BusinessRule(string property, string value)
    {
        property = Property;
        value = Rule;
    }
}