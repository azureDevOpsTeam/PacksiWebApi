using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class ApiDefinitionsEnum : SmartEnum<ApiDefinitionsEnum, byte>
{
    #region Fields

    public static ApiDefinitionsEnum Mobile = new(nameof(Mobile), 1);
    public static ApiDefinitionsEnum ExternalService = new(nameof(ExternalService), 2);
    public static ApiDefinitionsEnum Users = new(nameof(Users), 3);
    public static ApiDefinitionsEnum Admin = new(nameof(Admin), 4);

    #endregion Fields

    #region Methods

    #region Constructors

    public ApiDefinitionsEnum(string value, byte id) : base(value, id)
    {
    }

    #endregion Constructors

    #endregion Methods
}