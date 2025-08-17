namespace ApplicationLayer.DTOs.Advertisements
{
    public class CreateAdvertisementDto
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string MediaUrl { get; set; }

        public int PostType { get; set; }

        public int AdvertisementType { get; set; }

        public int? MaxViews { get; set; }

        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<AdScheduleDto> Schedules { get; set; }
    }
}