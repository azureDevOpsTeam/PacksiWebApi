namespace DomainLayer.Common.BaseEntities
{
    public interface IBaseEntity<T>
    {
        T Id { get; set; }

        bool IsActive { get; set; }

        bool IsDeleted { get; set; }
    }
}