namespace ApplicationLayer.DTOs.User
{
    public class UpdateUserProfileDto
    {
        public int CountryOfResidenceId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string Address { get; set; }

        public int? Gender { get; set; }

        public List<int> CityIds { get; set; }
    }
}