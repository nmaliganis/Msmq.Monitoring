namespace cbs.common.infrastructure.Queries;

public interface IQueryBuilder
{
}

public interface ISelectOptions<T> : IQueryBuilder
{
    string Select { get; }
}

public interface IExportOptions<T> : ISelectOptions<T>
{
    string Filter { get; }
    string Sort { get; }
    IQueryOptions<T> ToQueryOptions();
}

public interface IQueryOptions<T> : IExportOptions<T>
{
    uint? Skip { get; }

    uint? Top { get; }

    bool? Count { get; }


    IQueryOptions<T> NextPage();
    IQueryOptions<T> PreviousPage();
}