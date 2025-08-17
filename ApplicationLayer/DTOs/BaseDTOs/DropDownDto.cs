using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.BaseDTOs;

public class DropDownDto
{
    #region Properties

    public List<DropDownItemDto> ListItems { get; set; }

    #endregion Properties
}

public class DropDownItemDto
{
    #region Properties

    [JsonIgnore]
    public string Text { get; set; }

    public string Label { get => Text; }

    public string Value { get; set; }

    public string Icon { get; set; }

    #endregion Properties
}