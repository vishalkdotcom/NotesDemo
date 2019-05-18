using System.Collections.Generic;
using System.Threading.Tasks;
using NotesDemo.Entities;

namespace NotesDemo.Services
{
    public interface INoteService
    {
        Task<Note> Create(Note note);

        Task<Note> Get(string id);

        Task<List<Note>> GetByUserId(string userId, int page);

        Task<Note> Update(string id, Note note);

        Task Remove(string id);
    }

}