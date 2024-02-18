namespace cbs.common.infrastructure.Domain;

public class LinkDto
{
    private LinkDto()
    {
    }
    public LinkDto(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }

    public string Href { get; }
    public string Rel { get; }
    public string Method { get; }
}