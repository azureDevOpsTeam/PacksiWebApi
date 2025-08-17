#region Usings

using System.Text.Json.Serialization;

#endregion

namespace ApplicationLayer.DTOs.BaseDTOs
{
    public class CaptchaValueDto
    {
        public string CaptchaValue { get; set; }

        [JsonIgnore]
        public string DefaultCaptcha { get; set; }
    }
}