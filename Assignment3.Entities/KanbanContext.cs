namespace Assignment3.Entities;

public class KanbanContext : DbContext
{
    public KanbanContext(DbContextOptions<KanbanContext> options)
        : base(options)
    { }

    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
                    .Property(c => c.Name)
                    .HasMaxLength(100)
                    .IsRequired();

        modelBuilder.Entity<User>()
                    .HasIndex(c => c.Email)
                    .IsUnique();

        modelBuilder.Entity<User>()
                    .Property(c => c.Email)
                    .HasMaxLength(100)
                    .IsRequired();

        modelBuilder.Entity<Tag>()
                    .HasIndex(c => c.Name)
                    .IsUnique();

        modelBuilder.Entity<User>()
                    .Property(c => c.Name)
                    .HasMaxLength(50)
                    .IsRequired();

        modelBuilder.Entity<Task>()
                    .Property(c => c.Title)
                    .HasMaxLength(100)
                    .IsRequired();

        modelBuilder.Entity<Task>()
                    .Property(c => c.State)
                    .IsRequired();
    }

}
