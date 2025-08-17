#region Usings

using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using Microsoft.Extensions.Logging;

#endregion

namespace ApplicationLayer
{
    public class ServiceResult
    {
        #region Fields

        private string _message;

        #endregion Fields

        #region Properties

        public string ErrorCode { get; set; }

        public string Message
        {
            get => string.IsNullOrEmpty(_message) ? string.Empty : _message;
            set => _message = value;
        }

        public object Data { get; set; }

        public RequestStatus RequestStatus { get; set; }

        public NotificationType NotificationType => GetNotificationType(RequestStatus);

        #endregion Properties

        #region Methods

        private static NotificationType GetNotificationType(RequestStatus requestStatus)
        {
            switch (requestStatus)
            {
                case var SuccessfulRow when SuccessfulRow.Equals(RequestStatus.Successful):
                    return NotificationType.Success;

                case var failedRow when failedRow.Equals(RequestStatus.Failed):
                    return NotificationType.Error;

                default:
                    return NotificationType.Warning;
            }
        }

        public ServiceResult Failed(ILogger logger, Exception exception, string title)
        {
            logger.LogError(exception, title);
            return new ServiceResult()
            {
                RequestStatus = RequestStatus.Failed,
                Message = CommonMessages.Failed
            };
        }

        public ServiceResult Successful(object data = null)
        {
            return new ServiceResult()
            {
                Data = data,
                RequestStatus = RequestStatus.Successful,
                Message = CommonMessages.Successful
            };
        }

        public ServiceResult NotFound(object data = null)
        {
            return new ServiceResult()
            {
                Data = data,
                RequestStatus = RequestStatus.NotFound,
                Message = CommonMessages.NotFound
            };
        }

        public ServiceResult Duplicated(object data = null)
        {
            return new ServiceResult()
            {
                Data = data,
                RequestStatus = RequestStatus.Duplicated,
                Message = CommonMessages.Duplicate
            };
        }

        #endregion Methods
    }
}