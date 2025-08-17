namespace ApplicationLayer.DTOs.Requests
{
    public class DeliverableOriginLocationDto
    {
        public int? OriginCityId { get; set; }

        public string OriginDescription { get; set; }
    }

    public class DeliverableDestinationLocationDto
    {
        public int? DestinationCityId { get; set; }

        public string DestinationDescription { get; set; }
    }
}