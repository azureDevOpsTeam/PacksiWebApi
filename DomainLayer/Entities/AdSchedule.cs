using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class AdSchedule : BaseEntityModel, IAuditableEntity
    {
        public int AdvertisementId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan FromTime { get; set; }

        public TimeSpan ToTime { get; set; }

        public Advertisement Advertisement { get; set; }
    }
}