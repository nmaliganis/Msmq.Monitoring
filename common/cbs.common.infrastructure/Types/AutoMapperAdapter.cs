using AutoMapper;

namespace cbs.common.infrastructure.Types;

public class AutoMapperAdapter : IAutoMapper
{
    private readonly IMapper _mapper;

    public AutoMapperAdapter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public T Map<T>(object objectToMap)
    {
        return _mapper.Map<T>(objectToMap);
    }

    public TDest Map<TSource, TDest>(TSource objectSource, TDest objectDest)
    {
        return _mapper.Map(objectSource, objectDest);
    }
}