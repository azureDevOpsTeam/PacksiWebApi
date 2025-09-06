using DomainLayer.Common.Events;

namespace DomainLayer.Common.BaseEntities
{
    public abstract class BaseEntity<T> : IBaseEntity<T>
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public BaseEntity()
        {
            IsActive = true;
        }

        public virtual T Id { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}