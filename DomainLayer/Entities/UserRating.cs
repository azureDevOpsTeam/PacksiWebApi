using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class UserRating : BaseEntityModel, IAuditableEntity
{
    public int RateeUserAccountId { get; set; }

    public int RaterUserAccountId { get; set; }

    public int RequestId { get; set; }

    public int Rating { get; set; }

    public UserAccount RateeUserAccount { get; set; }

    public UserAccount RaterUserAccount { get; set; }

    public Request Request { get; set; }
}