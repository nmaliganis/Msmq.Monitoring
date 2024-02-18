namespace cbs.common.infrastructure.TypeHelpers;

public interface ITypeHelperService
{
    bool TypeHasProperties<T>(string fields);
}