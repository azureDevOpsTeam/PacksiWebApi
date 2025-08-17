#region Usings

using System.Text.Json.Serialization;

#endregion

namespace ApplicationLayer.DTOs.BaseDTOs
{
    public class MobileDto
    {
        #region Properties

        public string Mobile { get; set; }

        [JsonIgnore]
        public string Message { get; set; }

        [JsonIgnore]
        public string SecurityCode { get; set; }

        #endregion Properties
    }
}