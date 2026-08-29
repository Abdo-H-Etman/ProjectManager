using Domain.Common;

namespace Domain.Entities;

public class Comment : BaseEntity
{
    public Guid? TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsEdited { get; set; } = false;
    public DateTime EditedAt { get; set; }

    public Task? Task { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
}
