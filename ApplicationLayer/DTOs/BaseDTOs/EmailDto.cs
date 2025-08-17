#region Usings

using System.Text.Json.Serialization;

#endregion

namespace ApplicationLayer.DTOs.BaseDTOs
{
    public class EmailDto
    {
        #region Properties

        public string Email { get; set; }

        [JsonIgnore]
        public string SecurityCode { get; set; }

        #endregion
    }
}