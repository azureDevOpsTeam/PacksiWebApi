using ApplicationLayer.Extensions.SmartEnums;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.DTOs;

public class Result<T>
{
    #region Properties

    /// <summary>
    /// آیا عملیات موفقیت‌آمیز بوده است
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// آیا عملیات با خطا مواجه شده است
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// داده موفقیت‌آمیز
    /// </summary>
    public T Value { get; private set; }

    /// <summary>
    /// خطای رخ داده
    /// </summary>
    public Error Error { get; private set; }

    /// <summary>
    /// وضعیت درخواست
    /// </summary>
    public RequestStatus RequestStatus => IsSuccess ? Extensions.SmartEnums.RequestStatus.Successful : Error?.RequestStatus ?? Extensions.SmartEnums.RequestStatus.Failed;

    /// <summary>
    /// پیام نتیجه
    /// </summary>
    public string Message => IsSuccess ? "عملیات با موفقیت انجام شد" : Error?.Message ?? "خطای نامشخص";

    /// <summary>
    /// نوع اعلان
    /// </summary>
    public NotificationType NotificationType => IsSuccess ? NotificationType.Success : Error?.NotificationType ?? NotificationType.Error;

    #endregion Properties

    #region Constructors

    protected Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        Value = default(T);
        Error = error;
    }

    #endregion Constructors

    #region Static Methods

    /// <summary>
    /// ایجاد نتیجه موفقیت‌آمیز
    /// </summary>
    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    /// <summary>
    /// ایجاد نتیجه موفقیت‌آمیز بدون داده
    /// </summary>
    public static Result<T> Success()
    {
        return new Result<T>(default(T));
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق
    /// </summary>
    public static Result<T> Failure(Error error)
    {
        return new Result<T>(error);
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق با پیام
    /// </summary>
    public static Result<T> Failure(string message, string code = "ERROR")
    {
        return new Result<T>(new Error(code, message));
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق عمومی
    /// </summary>
    public static Result<T> GeneralFailure(string message)
    {
        return new Result<T>(Error.General(message));
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق احراز هویت
    /// </summary>
    public static Result<T> AuthenticationFailure(string message = "خطا در احراز هویت")
    {
        return new Result<T>(Error.Authentication(message));
    }

    /// <summary>
    /// ایجاد نتیجه یافت نشدن
    /// </summary>
    public static Result<T> NotFound(string message = "موردی یافت نشد")
    {
        return new Result<T>(Error.NotFound(message));
    }

    /// <summary>
    /// ایجاد نتیجه یافت نشدن
    /// </summary>
    public static Result<T> NotFoundFailure(string message = "موردی یافت نشد")
    {
        return new Result<T>(Error.NotFound(message));
    }

    /// <summary>
    /// ایجاد نتیجه خطای اعتبارسنجی
    /// </summary>
    public static Result<T> ValidationFailure(string message)
    {
        return new Result<T>(Error.Validation(message));
    }

    /// <summary>
    /// ایجاد نتیجه خطای تکراری
    /// </summary>
    public static Result<T> DuplicateFailure(string message = "رکورد تکراری")
    {
        return new Result<T>(Error.Duplicate(message));
    }

    /// <summary>
    /// ایجاد نتیجه خطای توکن منقضی
    /// </summary>
    public static Result<T> ExpiredTokenFailure(string message = "توکن منقضی شده")
    {
        return new Result<T>(Error.ExpiredToken(message));
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق از Exception
    /// </summary>
    public static Result<T> FromException(Exception exception, ILogger logger = null, string title = "خطای سیستم")
    {
        logger?.LogError(exception, title);
        return new Result<T>(Error.General($"{title}: {exception.Message}"));
    }

    #endregion Static Methods

    #region Methods

    /// <summary>
    /// تبدیل به ServiceResult برای سازگاری با کد موجود
    /// </summary>
    public ServiceResult ToServiceResult()
    {
        if (IsSuccess)
        {
            return new ServiceResult
            {
                RequestStatus = Extensions.SmartEnums.RequestStatus.Successful,
                Message = Message,
                Data = Value
            };
        }
        else
        {
            return new ServiceResult
            {
                RequestStatus = Error.RequestStatus,
                Message = Error.Message,
                ErrorCode = Error.Code,
                Data = Error.Details
            };
        }
    }

    /// <summary>
    /// اجرای عملیات در صورت موفقیت
    /// </summary>
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        if (IsFailure)
            return Result<TResult>.Failure(Error);

        try
        {
            var mappedValue = mapper(Value);
            return Result<TResult>.Success(mappedValue);
        }
        catch (Exception ex)
        {
            return Result<TResult>.GeneralFailure($"خطا در تبدیل داده: {ex.Message}");
        }
    }

    /// <summary>
    /// اجرای عملیات async در صورت موفقیت
    /// </summary>
    public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> mapper)
    {
        if (IsFailure)
            return Result<TResult>.Failure(Error);

        try
        {
            var mappedValue = await mapper(Value);
            return Result<TResult>.Success(mappedValue);
        }
        catch (Exception ex)
        {
            return Result<TResult>.GeneralFailure($"خطا در تبدیل داده: {ex.Message}");
        }
    }

    /// <summary>
    /// اجرای عملیات در صورت موفقیت و بازگشت Result
    /// </summary>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        if (IsFailure)
            return Result<TResult>.Failure(Error);

        try
        {
            return binder(Value);
        }
        catch (Exception ex)
        {
            return Result<TResult>.GeneralFailure($"خطا در اجرای عملیات: {ex.Message}");
        }
    }

    /// <summary>
    /// اجرای عملیات async در صورت موفقیت و بازگشت Result
    /// </summary>
    public async Task<Result<TResult>> BindAsync<TResult>(Func<T, Task<Result<TResult>>> binder)
    {
        if (IsFailure)
            return Result<TResult>.Failure(Error);

        try
        {
            return await binder(Value);
        }
        catch (Exception ex)
        {
            return Result<TResult>.GeneralFailure($"خطا در اجرای عملیات: {ex.Message}");
        }
    }

    #endregion Methods

    #region Implicit Operators

    /// <summary>
    /// تبدیل ضمنی از T به Result<T>
    /// </summary>
    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    /// <summary>
    /// تبدیل ضمنی از Error به Result<T>
    /// </summary>
    public static implicit operator Result<T>(Error error)
    {
        return Failure(error);
    }

    #endregion Implicit Operators
}

/// <summary>
/// کلاس Result بدون نوع generic برای عملیات بدون بازگشت داده
/// </summary>
public class Result : Result<object>
{
    private Result(object value) : base(value)
    {
    }

    private Result(Error error) : base(error)
    {
    }

    /// <summary>
    /// ایجاد نتیجه موفقیت‌آمیز
    /// </summary>
    public new static Result Success()
    {
        return new Result((object)null);
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق
    /// </summary>
    public new static Result Failure(Error error)
    {
        return new Result(error);
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق با پیام
    /// </summary>
    public new static Result Failure(string message, string code = "ERROR")
    {
        return new Result(new Error(code, message));
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق عمومی
    /// </summary>
    public new static Result GeneralFailure(string message)
    {
        return new Result(Error.General(message));
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق احراز هویت
    /// </summary>
    public new static Result AuthenticationFailure(string message = "خطا در احراز هویت")
    {
        return new Result(Error.Authentication(message));
    }

    /// <summary>
    /// ایجاد نتیجه یافت نشدن
    /// </summary>
    public new static Result NotFound(string message = "موردی یافت نشد")
    {
        return new Result(Error.NotFound(message));
    }

    /// <summary>
    /// ایجاد نتیجه یافت نشدن
    /// </summary>
    public new static Result NotFoundFailure(string message = "موردی یافت نشد")
    {
        return new Result(Error.NotFound(message));
    }

    /// <summary>
    /// ایجاد نتیجه خطای اعتبارسنجی
    /// </summary>
    public new static Result ValidationFailure(string message)
    {
        return new Result(Error.Validation(message));
    }

    /// <summary>
    /// ایجاد نتیجه خطای تکراری
    /// </summary>
    public new static Result DuplicateFailure(string message = "رکورد تکراری")
    {
        return new Result(Error.Duplicate(message));
    }

    /// <summary>
    /// ایجاد نتیجه خطای توکن منقضی
    /// </summary>
    public new static Result ExpiredTokenFailure(string message = "توکن منقضی شده")
    {
        return new Result(Error.ExpiredToken(message));
    }

    /// <summary>
    /// ایجاد نتیجه ناموفق از Exception
    /// </summary>
    public new static Result FromException(Exception exception, ILogger logger = null, string title = "خطای سیستم")
    {
        logger?.LogError(exception, title);
        return new Result(Error.General($"{title}: {exception.Message}"));
    }
}