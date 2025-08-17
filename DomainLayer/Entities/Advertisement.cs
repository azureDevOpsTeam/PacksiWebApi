using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Advertisement : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string MediaUrl { get; set; }

    public int PostType { get; set; }

    public int AdvertisementType { get; set; }

    public int? MaxViews { get; set; }

    public decimal Price { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime? PostedAt { get; set; }

    public int Status { get; set; }

    public UserAccount UserAccount { get; set; }

    public ICollection<AdSchedule> Schedules { get; set; } = [];
}