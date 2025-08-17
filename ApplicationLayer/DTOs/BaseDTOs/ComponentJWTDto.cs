namespace ApplicationLayer.DTOs.BaseDTOs
{
    public class ComponentJWTDto
    {
        public Guid ComponentGuid { get; set; }

        public List<int> AccessKind { get; set; }
    }
}