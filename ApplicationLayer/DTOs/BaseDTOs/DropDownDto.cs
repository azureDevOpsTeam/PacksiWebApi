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

    public string Text { get; set; }

    public string Label { get; set; }

    public string Value { get; set; }

    public string Icon { get; set; }

    #endregion Properties
}