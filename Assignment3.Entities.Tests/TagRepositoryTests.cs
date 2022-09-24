using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment3.Core;

namespace Assignment3.Entities.Tests;

public class TagRepositoryTests : IDisposable
{
    private KanbanContext _context;
    private ITagRepository _repository;

    public TagRepositoryTests() {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        var tag = new Tag() { Id = 1, Name = "Tag1" };
        context.Tags.AddRange(tag, new Tag() { Id = 2, Name = "Tag2" });
        context.Users.AddRange(new User { Id = 1, Name = "User1", Email = "user1@itu.dk" }, new User { Id = 2, Name = "User2", Email = "user2@itu.dk" });
        context.Tasks.AddRange(new Task { Id = 1, Title = "title1", State = State.New, Tags = new List<Tag>(){tag}}, new Task { Id = 2, Title = "title2", State = State.Active});
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void Create_given_Tag_returns_Created_with_Tag()
    {
        // Given
        var (response, id) = _repository.Create(new TagCreateDTO("Tag3"));

        // When
        response.Should().Be(Response.Created);

        // Then
        id.Should().Be(3);
    }

    [Fact]
    public void Create_given_existing_Tag_returns_Conflict_with_existing_Tag()
    {
        var (response, id) = _repository.Create(new TagCreateDTO("Tag1"));

        response.Should().Be(Response.Conflict);

        id.Should().Be(1);
    }

    [Fact]
    public void Delete_non_existing_tag_should_give_not_found()
    {
        // Given
        var response = _repository.Delete(4, false);
    
        // Then
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void Delete_existing_tag_in_use_without_force_should_give_conflict()
    {
        // Given
        var response = _repository.Delete(1, false);
    
        // Then
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Delete_existing_tag_in_use_with_force_should_give_deleted()
    {
        // Given
        var response = _repository.Delete(1, true);
    
        // Then
        response.Should().Be(Response.Deleted);
    }
}
