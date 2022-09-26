using Assignment3.Core;

namespace Assignment3.Entities;

public class TaskRepository : ITaskRepository
{
    private readonly KanbanContext _context;

    public TaskRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {
        var entity = _context.Tasks.FirstOrDefault(c => c.Id == task.AssignedToId);
        Response response;

        if (entity is null)
        {
            entity = new Task() { Title = task.Title };

            if (task.AssignedToId is not null) entity.Id = (int)task.AssignedToId;
            if (task.Description is not null) entity.Description = (string)task.Description;
            foreach (var t in task.Tags)
            {
                var tag = _context.Tags.FirstOrDefault(c => c.Name == t);
                if (tag is not null) entity.Tags.Add(tag);
            }

            _context.Tasks.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);
    }

    public Response Delete(int taskId)
    {
        var entity = _context.Tasks.FirstOrDefault(c => c.Id == taskId);
        Response response;

        if (entity is null)
            response = Response.NotFound;
        else if (entity.State == State.New)
        {
            _context.Tasks.Remove(entity);
            _context.SaveChanges();
            response = Response.Deleted;
        }
        else if (entity.State == State.Active)
        {
            entity.State = State.Removed;
            response = Response.BadRequest; // dont know if this is the right response, couldnt tell from the business rules
        }
        else
        {
            response = Response.Conflict;
        }
        return response;
    }

    public TaskDetailsDTO Read(int taskId)
    {
        var entity = _context.Tasks.FirstOrDefault(c => c.Id == taskId);

        if (entity is null) return null;
        var list = new List<string>();
        foreach (var tag in entity.Tags)
            list.Add(tag.Name);

        return new TaskDetailsDTO(taskId, entity.Title, entity.Description, entity.Created, entity.AssignedTo is null ? null : entity.AssignedTo.Name, list, entity.State, entity.StateUpdated);

    }

    public IReadOnlyCollection<TaskDTO> ReadAll()
    {
        var list = new List<TaskDTO>();
        var entity = _context.Tasks;
        foreach (var e in entity)
        {
            var tags = new List<string>();
            foreach (var t in e.Tags)
                tags.Add(t.Name);
            list.Add(new TaskDTO(e.Id, e.Title, e.AssignedTo is null ? null : e.AssignedTo.Name, tags, e.State));
        }
        return list;
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByState(Core.State state)
    {
        var list = new List<TaskDTO>();
        var entity = _context.Tasks;
        foreach (var e in entity)
        {
            if (e.State == state)
            {
                var tags = new List<string>();
                foreach (var t in e.Tags)
                    tags.Add(t.Name);
                list.Add(new TaskDTO(e.Id, e.Title, e.AssignedTo is null ? null : e.AssignedTo.Name, tags, e.State));
            }
        }
        return list;
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
    {
        var shouldBeAdded = false;
        var list = new List<TaskDTO>();
        var entity = _context.Tasks;
        foreach (var e in entity)
        {
            shouldBeAdded = false;
            var tags = new List<string>();
            foreach (var t in e.Tags) {
                tags.Add(t.Name);
                if (t.Name == tag) shouldBeAdded = true;
            }
            if (shouldBeAdded)
                list.Add(new TaskDTO(e.Id, e.Title, e.AssignedTo is null ? null : e.AssignedTo.Name, tags, e.State));
        }
        return list;
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
    {
        var list = new List<TaskDTO>();
        var entity = _context.Tasks;
        foreach (var e in entity)
        {
            if (e.AssignedTo is not null && e.AssignedTo.Id == userId)
            {
                var tags = new List<string>();
                foreach (var t in e.Tags)
                    tags.Add(t.Name);
                list.Add(new TaskDTO(e.Id, e.Title, e.AssignedTo is null ? null : e.AssignedTo.Name, tags, e.State));
            }
        }
        return list;
    }

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
    {
        var list = new List<TaskDTO>();
        var entity = _context.Tasks;
        foreach (var e in entity)
        {
            if (e.State == State.Removed)
            {
                var tags = new List<string>();
                foreach (var t in e.Tags)
                    tags.Add(t.Name);
                list.Add(new TaskDTO(e.Id, e.Title, e.AssignedTo is null ? null : e.AssignedTo.Name, tags, e.State));
            }
        }
        return list;
    }

    public Response Update(TaskUpdateDTO task)
    {
        var entity = _context.Tasks.FirstOrDefault(c => c.Id == task.Id);

        if (entity is null) return Response.NotFound;
        else {
            entity.Title = task.Title;
            entity.UpdateState(task.State);
            entity.AssignedTo = _context.Users.FirstOrDefault(c => c.Id == task.AssignedToId);
            entity.Description = task.Description;
            entity.Tags = new List<Tag>();
            foreach(var s in task.Tags) {
                var tag = _context.Tags.FirstOrDefault(c => c.Name == s);
                if (tag is not null)
                    entity.Tags.Add(tag);
            }

            _context.SaveChanges();
            return Response.Updated;
        }
    }

}
