using ApplicationLayer.DTOs;
using ApplicationLayer.Extensions.SmartEnums;

namespace ApplicationLayer.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// تبدیل ServiceResult به Result<T>
    /// </summary>
    public static Result<T> ToResult<T>(this ServiceResult serviceResult)
    {
        if (serviceResult.RequestStatus.Equals(RequestStatus.Successful))
        {
            return Result<T>.Success((T)serviceResult.Data);
        }
        else
        {
            var error = new Error(
                serviceResult.ErrorCode ?? "SERVICE_ERROR",
                serviceResult.Message,
                serviceResult.RequestStatus
            );
            return Result<T>.Failure(error);
        }
    }

    /// <summary>
    /// تبدیل ServiceResult به Result (بدون نوع)
    /// </summary>
    public static Result ToResult(this ServiceResult serviceResult)
    {
        if (serviceResult.RequestStatus.Equals(RequestStatus.Successful))
        {
            return Result.Success();
        }
        else
        {
            var error = new Error(
                serviceResult.ErrorCode ?? "SERVICE_ERROR",
                serviceResult.Message,
                serviceResult.RequestStatus
            );
            return Result.Failure(error);
        }
    }

    /// <summary>
    /// تبدیل Result<T> به ServiceResult
    /// </summary>
    public static ServiceResult ToServiceResult<T>(this Result<T> result)
    {
        return result.ToServiceResult();
    }

    /// <summary>
    /// اجرای عملیات با مدیریت خطا و بازگشت Result
    /// </summary>
    public static Result<T> Execute<T>(Func<T> operation)
    {
        try
        {
            var result = operation();
            return Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<T>.GeneralFailure($"خطا در اجرای عملیات: {ex.Message}");
        }
    }

    /// <summary>
    /// اجرای عملیات async با مدیریت خطا و بازگشت Result
    /// </summary>
    public static async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            var result = await operation();
            return Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<T>.GeneralFailure($"خطا در اجرای عملیات: {ex.Message}");
        }
    }

    /// <summary>
    /// اجرای عملیات بدون بازگشت داده با مدیریت خطا
    /// </summary>
    public static Result Execute(Action operation)
    {
        try
        {
            operation();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.GeneralFailure($"خطا در اجرای عملیات: {ex.Message}");
        }
    }

    /// <summary>
    /// اجرای عملیات async بدون بازگشت داده با مدیریت خطا
    /// </summary>
    public static async Task<Result> ExecuteAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.GeneralFailure($"خطا در اجرای عملیات: {ex.Message}");
        }
    }

    /// <summary>
    /// ترکیب چندین Result و بازگشت اولین خطا یا موفقیت
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return result;
        }
        return Result.Success();
    }

    /// <summary>
    /// ترکیب چندین Result<T> و بازگشت لیست مقادیر موفق یا اولین خطا
    /// </summary>
    public static Result<List<T>> CombineValues<T>(params Result<T>[] results)
    {
        var values = new List<T>();

        foreach (var result in results)
        {
            if (result.IsFailure)
                return Result<List<T>>.Failure(result.Error);

            values.Add(result.Value);
        }

        return Result<List<T>>.Success(values);
    }

    /// <summary>
    /// اعمال شرط بر روی Result
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        if (result.IsFailure)
            return result;

        if (!predicate(result.Value))
            return Result<T>.Failure(error);

        return result;
    }

    /// <summary>
    /// اعمال شرط بر روی Result با پیام خطا
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
    {
        return result.Ensure(predicate, Error.General(errorMessage));
    }

    /// <summary>
    /// تبدیل Result<T> به HandlerResult
    /// </summary>
    public static HandlerResult ToHandlerResult<T>(this Result<T> result)
    {
        return new HandlerResult
        {
            RequestStatus = result.RequestStatus,
            ObjectResult = result.IsSuccess ? result.Value : null,
            Message = result.Message
        };
    }

    /// <summary>
    /// تبدیل Result به HandlerResult
    /// </summary>
    public static HandlerResult ToHandlerResult(this Result result)
    {
        return new HandlerResult
        {
            RequestStatus = result.RequestStatus,
            ObjectResult = null,
            Message = result.Message
        };
    }
}