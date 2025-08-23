namespace ApplicationLayer.DTOs.User
{
    public class PreferredLocationDto
    {
        public int? CountryOfResidenceId { get; set; }

        public List<int> CityIds { get; set; }
    }
}