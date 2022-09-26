using Assignment3.Core;
namespace Assignment3.Entities;

public class Task
{

    public Task() {
        Created = DateTime.UtcNow;
        StateUpdated = DateTime.UtcNow;
    }

    public virtual int Id { get; set; }

    public virtual string Title { get; set; } = null!;

    public virtual User? AssignedTo { get; set; } = null;

    public virtual string? Description { get; set; }

    public virtual State State { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>{};

    public virtual DateTime Created {get; }

    public virtual DateTime StateUpdated {get; set; }

    public void UpdateState(State state) {
        State = state;
        StateUpdated = DateTime.UtcNow;
    }
}
