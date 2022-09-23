namespace Assignment3.Entities;

public class Tag
{
    public virtual int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public virtual string Name { get; set; }

    public virtual ICollection<Task> Tasks { get; set; }
}
