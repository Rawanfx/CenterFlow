namespace CenterFlow.Domain.Common
{
    public abstract class BaseEntity
    {
        private readonly List<IDomainEvent> domainEvents = new();
        protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
            domainEvents.Add(domainEvent);
        public void ClearDomainEvents()=>
            domainEvents.Clear();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();
    }
}
