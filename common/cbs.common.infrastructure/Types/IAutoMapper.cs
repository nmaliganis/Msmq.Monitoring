namespace cbs.common.infrastructure.Types;

public interface IAutoMapper
{
    T Map<T>(object objectToMap);
    TDest Map<TSource, TDest>(TSource objectSource, TDest objectDest);
}