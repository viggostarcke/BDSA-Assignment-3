namespace Assignment3.Entities;

public class Tag
{
    public virtual int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public ICollection<Task> Tasks { get; set; } = new List<Task>{};
}
