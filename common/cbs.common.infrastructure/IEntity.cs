namespace cbs.common.infrastructure
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }
}