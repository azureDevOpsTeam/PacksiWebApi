namespace ApplicationLayer.DTOs.Advertisements;

public class GetAllFilterDto
{
    public int Status { get; set; }

    public PaginationDto Pagination { get; set; }
}