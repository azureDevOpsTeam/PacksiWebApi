namespace ApplicationLayer.DTOs.User;

public class UserInvitedResultDto
{
    public int UserAccountId { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public bool ConfirmEmail { get; set; }

    public bool ConfirmPhoneNumber { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public string FullName { get; set; }

    public string Address { get; set; }

    public string Company { get; set; }
}