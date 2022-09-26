using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment3.Core;

namespace Assignment3.Entities.Tests;

public class TaskRepositoryTests : IDisposable
{
    private KanbanContext _context;
    private ITaskRepository _repository;

    public TaskRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        var tag = new Tag() { Id = 1, Name = "Tag1" };
        var user = new User { Id = 1, Name = "User1", Email = "user1@itu.dk" };
        context.Tags.AddRange(tag, new Tag() { Id = 2, Name = "Tag2" });
        context.Users.AddRange(user, new User { Id = 2, Name = "User2", Email = "user2@itu.dk" });
        context.Tasks.AddRange(new Task { Id = 1, Title = "title1", AssignedTo = user, Description = "descrption", State = State.New, Tags = new List<Tag>() { tag } }, new Task { Id = 2, Title = "title2", State = State.Active }, new Task { Id = 3, Title = "title3", State = State.Resolved });
        context.SaveChanges();

        _context = context;
        _repository = new TaskRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void Create_given_Task_returns_Created_with_Task()
    {
        // Given
        var (response, id) = _repository.Create(new TaskCreateDTO("newTask", 5, "description", new List<string>()));

        var entity = _context.Tasks.FirstOrDefault(c => c.Id == id);

        // Then
        id.Should().Be(5);
        response.Should().Be(Response.Created);

        entity.Created.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
        entity.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));

    }

    [Fact]
    public void Create_given_existing_Task_returns_Conflict_with_existing_Task()
    {
        var (response, id) = _repository.Create(new TaskCreateDTO("newTask", 1, "description", new List<string>()));

        id.Should().Be(1);
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Delete_non_existing_Task_return_notfound()
    {
        // Given
        var response = _repository.Delete(4);

        // Then
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void Delete_existing_Task_State_new_return_deleted()
    {
        // Given
        var response = _repository.Delete(1);

        // Then
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public void Delete_existing_Task_state_active_return_badrequest()
    {
        // Given
        var response = _repository.Delete(2);

        // Then
        response.Should().Be(Response.BadRequest);
    }

    [Fact]
    public void Delete_existing_Task_state_resolved_return_conflict()
    {
        // Given
        var response = _repository.Delete(3);

        // Then
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Read_should_return_null_for_non_existing()
    {
        // Given
        var entity = _repository.Read(6);

        // Then
        entity.Should().BeNull();
    }

    [Fact]
    public void Read_should_return_TaskDetailsDTO_for_existing_task()
    {
        // Given
        var entity = _repository.Read(1);

        // Then
        entity.Id.Should().Be(1);
        entity.Title.Should().Be("title1");
        entity.Description.Should().Be("descrption");
        entity.AssignedToName.Should().Be("User1");
        entity.Created.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
        entity.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
        entity.State.Should().Be(State.New);
        entity.Tags.Should().BeEquivalentTo(new List<string>() { "Tag1" });
    }

    [Fact]
    public void ReadAll_should_return_all_tasks()
    {
        // Given
        var entity = _repository.ReadAll();

        // Then
        entity.Should().BeEquivalentTo(new List<TaskDTO>() { new TaskDTO(1, "title1", "User1", new List<string>() { "Tag1" }, State.New), new TaskDTO(2, "title2", null, new List<string> { }, State.Active), new TaskDTO(3, "title3", null, new List<string> { }, State.Resolved) });

    }

    [Fact]
    public void ReadAllbyState_should_return_all_tasks_with_given_state()
    {
        // Given
        var entity = _repository.ReadAllByState(State.Active);

        // Then
        entity.Should().BeEquivalentTo(new List<TaskDTO>() { new TaskDTO(2, "title2", null, new List<string> { }, State.Active) });

    }

    [Fact]
    public void ReadAllbyTag_should_return_all_tasks_with_given_tag()
    {
        // Given
        var entity = _repository.ReadAllByTag("Tag1");

        // Then
        entity.Should().BeEquivalentTo(new List<TaskDTO>() { new TaskDTO(1, "title1", "User1", new List<string>() { "Tag1" }, State.New) });

    }

    [Fact]
    public void ReadAllbyUser_should_return_all_tasks_with_given_user()
    {
        // Given
        var entity = _repository.ReadAllByUser(1);

        // Then
        entity.Should().BeEquivalentTo(new List<TaskDTO>() { new TaskDTO(1, "title1", "User1", new List<string>() { "Tag1" }, State.New) });

    }

    [Fact]
    public void ReadAllRemoved_should_return_all_tasks_with_given_user()
    {
         // Given
        var entity = _repository.ReadAllRemoved();

        // Then
        entity.Should().BeEquivalentTo(new List<TaskDTO>());

    }

    [Fact]
    public void Update_existing_task_should_return_updated()
    {
        // Given
        var response = _repository.Update(new TaskUpdateDTO(1, "new", null, null, new List<string>(), State.New));

        // Then
        response.Should().Be(Response.Updated);

    }

    [Fact]
    public void Update_non_existing_task_should_return_notfound()
    {
        // Given
        var response = _repository.Update(new TaskUpdateDTO(5, "new", null, null, new List<string>(), State.New));

        // Then
        response.Should().Be(Response.NotFound);
    }


}
