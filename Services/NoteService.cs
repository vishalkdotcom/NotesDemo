using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotesDemo.Entities;

namespace NotesDemo.Services
{
    public class NoteService : INoteService
    {
        private readonly NotesDbContext _dbContext;

        public NoteService(NotesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Note> Create(Note note)
        {
            note.CreatedAt = GetEpochTime();

            await _dbContext.Notes.AddAsync(note);
            await _dbContext.SaveChangesAsync();
            return note;
        }

        public async Task<Note> Get(string id)
        {
            return await GetNoteById(id);
        }

        public async Task<List<Note>> GetAllByUserId(string userId)
        {
            return await _dbContext.Notes.Where(i => i.UserId == userId).ToListAsync();
        }

        public async Task Remove(string id)
        {
            var noteToRemove = await GetNoteById(id);
            if (noteToRemove != null)
            {
                _dbContext.Notes.Remove(noteToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Note> Update(string id, Note note)
        {
            var noteToUpdate = await GetNoteById(id);
            if (noteToUpdate != null)
            {
                noteToUpdate.Title = note.Title;
                noteToUpdate.Content = note.Content;
                noteToUpdate.IsPinned = note.IsPinned;
                note.CreatedAt = GetEpochTime();

                _dbContext.Notes.Update(noteToUpdate);
                await _dbContext.SaveChangesAsync();
            }
            return noteToUpdate;
        }

        private async Task<Note> GetNoteById(string id)
        {
            return await _dbContext.Notes.SingleOrDefaultAsync(i => i.Id == id);
        }

        private int GetEpochTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            return secondsSinceEpoch;
        }
    }
}