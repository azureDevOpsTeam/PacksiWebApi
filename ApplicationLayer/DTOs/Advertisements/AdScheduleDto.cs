namespace ApplicationLayer.DTOs.Advertisements
{
    public class AdScheduleDto
    {
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan FromTime { get; set; }

        public TimeSpan ToTime { get; set; }
    }
}