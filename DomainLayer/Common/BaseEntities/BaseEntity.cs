namespace DomainLayer.Common.BaseEntities
{
    public abstract class BaseEntity<T> : IBaseEntity<T>
    {
        public BaseEntity()
        {
            IsActive = true;
        }

        public virtual T Id { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}