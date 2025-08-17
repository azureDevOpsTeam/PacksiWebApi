using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class RequestSelection : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public int RequestId { get; set; }

    public int Status { get; set; }

    public UserAccount UserAccount { get; set; }

    public Request Request { get; set; }
}