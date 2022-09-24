using Microsoft.Data.Sqlite;
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
            entity = new Tag(){Name = tag.Name};

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
        var entity = _context.Tags.FirstOrDefault(c => c.Id == tagId);
        Response response;

        if (entity is not null && force == false) 
            response = Response.Conflict;
        else if (entity is null) 
            response = Response.NotFound;
        else
        {
            _context.Tags.Remove(entity);
            _context.SaveChanges();

            response = Response.Deleted;
        }

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
