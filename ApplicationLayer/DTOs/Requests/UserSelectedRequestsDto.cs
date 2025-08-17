namespace ApplicationLayer.DTOs.Requests;

public class MyPostedSelectedDto
{
    public int RequestId { get; set; }

    public int RequestSelectionId { get; set; }

    public int SelectorUserAccountId { get; set; }

    public string SelectorFirstName { get; set; }

    public string SelectorLastName { get; set; }

    public string SelectorFullName
    {
        get
        {
            return $"{SelectorFirstName} {SelectorLastName}";
        }
    }

    public int LastStatus { get; set; }

    public string LastStatusStr { get; set; }
}