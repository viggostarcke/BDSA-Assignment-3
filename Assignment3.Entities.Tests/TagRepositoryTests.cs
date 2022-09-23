using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities.Tests;

public class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests() {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        context.Tags.AddRange(new Tag() { Id = 1, Name = "Tag1" }, new Tag() { Id = 2, Name = "Tag2" });
        context.Users.AddRange(new User { Id = 1, Name = "User1", Email = "user1@itu.dk" }, new User { Id = 2, Name = "User2", Email = "user2@itu.dk" });
        context.Tasks.AddRange(new Task { Id = 1, Title = "title1", State = State.New}, new Task { Id = 2, Title = "title2", State = State.Active});
        context.SaveChanges();

        _context = context;
        //_repository = new TagRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
