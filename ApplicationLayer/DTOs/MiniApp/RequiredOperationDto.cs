using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.MiniApp
{
    public class RequiredOperationDto
    {
        [JsonIgnore]
        public int UserAccountId { get; set; }

        [JsonIgnore]
        public int? CountryOfResidenceId { get; set; }

        public bool SetPreferredLocation { get; set; }

        [JsonIgnore]
        public string DisplayName { get; set; }

        [JsonIgnore]
        public string FirstName { get; set; }

        [JsonIgnore]
        public string LastName { get; set; }

        public bool ConfirmPhoneNumber { get; set; }

        public bool HasCompletedProfile
        {
            get
            {
                return (CountryOfResidenceId > 0 && SetPreferredLocation && FirstName is not null && LastName is not null);
            }
        }

        public bool IsAcceptRules { get; set; }
    }
}