using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesDemo.Entities;
using NotesDemo.Models;
using NotesDemo.Services;

namespace NotesDemo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        const int MAX_ITEMS = 10;
        private readonly INoteService _noteService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotesController(INoteService noteService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _noteService = noteService;
            _mapper = mapper;
        }

        // GET: api/notes
        public async Task<ActionResult<List<NoteModel>>> Get([FromQuery]int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var notes = (await _noteService.GetAllByUserId(user.Id))
                            .Skip((page - 1) * MAX_ITEMS)
                            .Take(MAX_ITEMS)
                            .ToList();
                return _mapper.Map<List<NoteModel>>(notes);
            }

            return BadRequest();
        }

        // GET: api/notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NoteModel>> GetNote(string id)
        {
            var note = await _noteService.Get(id);
            if (note == null)
            {
                return NotFound();
            }

            return _mapper.Map<NoteModel>(note);
        }

        // POST: api/notes
        [HttpPost]
        public async Task<ActionResult<NoteModel>> Create([FromBody]NoteModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var note = _mapper.Map<Note>(model);
                    note.User = user;

                    note = await _noteService.Create(note);
                    return CreatedAtAction(nameof(GetNote), new { id = note.Id }, _mapper.Map<NoteModel>(note));
                }
            }

            return BadRequest();
        }

        // PUT: api/notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NoteModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(id))
            {
                var note = await _noteService.Get(id);
                if (note == null)
                {
                    return NotFound();
                }

                var noteToUpdate = _mapper.Map<Note>(model);
                noteToUpdate.Id = id;

                note = await _noteService.Update(id, noteToUpdate);
                return Ok(_mapper.Map<NoteModel>(note));
            }

            return BadRequest();
        }

        // DELETE: api/notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(id))
            {
                var note = await _noteService.Get(id);
                if (note == null)
                {
                    return NotFound();
                }

                await _noteService.Remove(note.Id);
                return NoContent();
            }

            return BadRequest();
        }
    }
}
