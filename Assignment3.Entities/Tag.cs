namespace Assignment3.Entities;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public ICollection<Task> Tasks { get; set; }
}
