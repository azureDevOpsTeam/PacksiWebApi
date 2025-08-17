using ApplicationLayer.Extensions.SmartEnums;

namespace ApplicationLayer.DTOs;

public class Error
{
    #region Properties

    /// <summary>
    /// کد خطا
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// پیام خطا
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// نوع درخواست
    /// </summary>
    public RequestStatus RequestStatus { get; set; }

    /// <summary>
    /// نوع اعلان
    /// </summary>
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// جزئیات اضافی خطا
    /// </summary>
    public object Details { get; set; }

    #endregion Properties

    #region Constructors

    public Error()
    {
    }

    public Error(string code, string message, RequestStatus requestStatus = null)
    {
        Code = code;
        Message = message;
        RequestStatus = requestStatus ?? RequestStatus.Failed;
        NotificationType = GetNotificationType(RequestStatus);
    }

    #endregion Constructors

    #region Methods

    private static NotificationType GetNotificationType(RequestStatus requestStatus)
    {
        switch (requestStatus)
        {
            case var successfulRow when successfulRow.Equals(RequestStatus.Successful):
                return NotificationType.Success;

            case var failedRow when failedRow.Equals(RequestStatus.Failed):
                return NotificationType.Error;

            case var authFailedRow when authFailedRow.Equals(RequestStatus.AuthenticationFailed):
                return NotificationType.Error;

            case var notFoundRow when notFoundRow.Equals(RequestStatus.NotFound):
                return NotificationType.Warning;

            case var validationFailedRow when validationFailedRow.Equals(RequestStatus.ValidationFailed):
                return NotificationType.Warning;

            default:
                return NotificationType.Warning;
        }
    }

    /// <summary>
    /// ایجاد خطای عمومی
    /// </summary>
    public static Error General(string message, string code = "GENERAL_ERROR")
    {
        return new Error(code, message, RequestStatus.Failed);
    }

    /// <summary>
    /// ایجاد خطای احراز هویت
    /// </summary>
    public static Error Authentication(string message = "خطا در احراز هویت", string code = "AUTH_ERROR")
    {
        return new Error(code, message, RequestStatus.AuthenticationFailed);
    }

    /// <summary>
    /// ایجاد خطای یافت نشدن
    /// </summary>
    public static Error NotFound(string message = "موردی یافت نشد", string code = "NOT_FOUND")
    {
        return new Error(code, message, RequestStatus.NotFound);
    }

    /// <summary>
    /// ایجاد خطای اعتبارسنجی
    /// </summary>
    public static Error Validation(string message, string code = "VALIDATION_ERROR")
    {
        return new Error(code, message, RequestStatus.ValidationFailed);
    }

    /// <summary>
    /// ایجاد خطای تکراری
    /// </summary>
    public static Error Duplicate(string message = "رکورد تکراری", string code = "DUPLICATE_ERROR")
    {
        return new Error(code, message, RequestStatus.Duplicated);
    }

    /// <summary>
    /// ایجاد خطای توکن منقضی
    /// </summary>
    public static Error ExpiredToken(string message = "توکن منقضی شده", string code = "EXPIRED_TOKEN")
    {
        return new Error(code, message, RequestStatus.ExpiredToken);
    }

    #endregion Methods
}