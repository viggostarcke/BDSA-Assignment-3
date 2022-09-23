namespace Assignment3.Entities;

public class User
{
    public virtual int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public virtual string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public virtual string Email { get; set; }

    public virtual List<Task> Tasks { get; set; }
}
