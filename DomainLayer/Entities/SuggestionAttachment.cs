using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class SuggestionAttachment : BaseEntityModel, IAuditableEntity
{
    public int SuggestionId { get; set; }

    public string FilePath { get; set; }

    public Suggestion Suggestion { get; set; }
}