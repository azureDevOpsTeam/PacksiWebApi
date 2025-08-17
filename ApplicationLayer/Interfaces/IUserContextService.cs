namespace ApplicationLayer.Interfaces
{
    public interface IUserContextService
    {
        int? UserId { get; }

        string UserIpAddress { get; }

        string UserFirstName { get; }

        string UserLastName { get; }

        string UserFullName { get; }

        string UserDisplayName { get; }

        string UserName { get; }
    }
}