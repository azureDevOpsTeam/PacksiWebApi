using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.BaseDTOs
{
    public class DropDownTreeDto
    {
        #region Properties

        public List<DropDownItemTreeDto> ListItems { get; set; }

        #endregion Properties
    }

    public class DropDownItemTreeDto
    {
        #region Properties

        public string Text { get; set; }

        public string Label { get => Text; }

        public string Value { get; set; }

        public string Icon { get; set; }

        public List<DropDownItemTreeDto> Children { get; set; } = [];

        #endregion Properties
    }
}