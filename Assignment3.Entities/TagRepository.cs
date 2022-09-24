using Assignment3.Core;

namespace Assignment3.Entities;
public class TagRepository : ITagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        _context = context;
    }

    (Response Response, int TagId) ITagRepository.Create(TagCreateDTO tag)
    {
        var entity = _context.Tags.FirstOrDefault(c => c.Name == tag.Name);
        Response response;

        if (entity is null)
        {
            entity = new Tag() { Name = tag.Name };

            _context.Tags.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);
    }

    Response ITagRepository.Delete(int tagId, bool force)
    {
        var tagExisting = _context.Tags.FirstOrDefault(c => c.Id == tagId);
        Response response;

        var tasks = _context.Tasks;
        tasks.Find(tagId);
        Tag tagInUse = null;
        foreach (var t in tasks) foreach (var tag in t.Tags) if (tag.Id == tagId) tagInUse = tag;

        if (tagExisting is null)
            response = Response.NotFound;
        else if (tagInUse is null || force)
        {
            _context.Tags.Remove(tagExisting);
            _context.SaveChanges();

            if (tagInUse is not null) foreach (var task in _context.Tasks) task.Tags.Remove(tagExisting);

            response = Response.Deleted;
        }
        else response = Response.Conflict;

        return response;
    }

    TagDTO ITagRepository.Read(int tagId)
    {
        var entity = _context.Tags.FirstOrDefault(c => c.Id == tagId);
        if (entity is null) return null;
        return new TagDTO(tagId, entity.Name);
    }

    IReadOnlyCollection<TagDTO> ITagRepository.ReadAll()
    {
        // var entity = _context.Tags;
        // foreach (var e in entity) {
        //     yield return new TagDTO(e.Id, e.Name);
        // }

        // IReadOnlyCollection<TagDTO> r = new IReadOnlyCollection
        throw new NotImplementedException();

    }

    Response ITagRepository.Update(TagUpdateDTO tag)
    {

        throw new NotImplementedException();
    }
}
