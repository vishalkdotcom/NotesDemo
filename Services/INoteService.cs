using System.Collections.Generic;
using System.Threading.Tasks;
using NotesDemo.Entities;

namespace NotesDemo.Services
{
    public interface INoteService
    {
        Task<Note> Create(Note note);

        Task<Note> Get(string id);

        Task<List<Note>> GetAllByUserId(string userId);

        Task<Note> Update(string id, Note note);
        
        Task Remove(string id);
    }

}