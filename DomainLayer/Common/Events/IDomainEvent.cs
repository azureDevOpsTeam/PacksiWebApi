using MediatR;

namespace DomainLayer.Common.Events
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}