#region Usings

using Ardalis.SmartEnum;

#endregion

namespace ApplicationLayer.Extensions.SmartEnums;

public sealed class ValidationMethodEnum(string name, int value) : SmartEnum<ValidationMethodEnum>(name, value)
{
    #region Field

    public static ValidationMethodEnum UserInformation = new("اطلاعات کاربری", 1);

    public static ValidationMethodEnum OneTimePasswordMobile = new("رمز یکبار مصرف موبایل", 2);

    public static ValidationMethodEnum OneTimePasswordEmail = new("رمز یکبار مصرف ایمیل", 3);

    #endregion Field
}