namespace ApplicationLayer.DTOs.User;

public class UserInfoDto
{
    public int UserAccountId { get; set; }

    public int? CountryOfResidenceId { get; set; }

    public bool SetPreferredLocation { get; set; }

    public string DisplayName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool ConfirmPhoneNumber { get; set; }

    public bool HasCompletedProfile
    {
        get
        {
            return (CountryOfResidenceId > 0 && SetPreferredLocation && FirstName is not null && LastName is not null);
        }
    }
}