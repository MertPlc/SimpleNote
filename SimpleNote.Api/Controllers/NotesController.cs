using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleNote.Api.Data;
using SimpleNote.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleNote.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Note>> GetNotes()
        {
            return await _context.Notes.OrderByDescending(x => x.Time).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            Note note = await _context.Notes.FindAsync(id);

            if (note == null)
                return NotFound();

            return note;
        }

        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(PostNoteDto dto)  //post eklerken id'ye ihtiyacımız yok ama Note'tan id gelecek. Bu durum overposting olarak gecer. DTO olarak cozerız
        {
            if (ModelState.IsValid)
            {
                Note note = new Note()
                {
                    Title = dto.Title,
                    Content = dto.Content
                };
                _context.Add(note);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);  // api/notes/4 ile de note içindeki veriye erişebilirsin
            }

            return BadRequest(ModelState);  // hatalı olan modelstate'ı dondurunce 404 dondurur ve hata mesajlarını oto verir
        }

        // PUT: api/Notes/3  
        [HttpPut("{id}")]
        public async Task<ActionResult<Note>> PutNote(int id, PutNoteDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                Note note = await _context.Notes.FindAsync(id);

                if (note == null)
                    return NotFound();

                note.Title = dto.Title;
                note.Content = dto.Content;
                note.Time = DateTime.Now;
                await _context.SaveChangesAsync();
                return note;
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            Note note = await _context.Notes.FindAsync(id);

            if (note == null) return NotFound();

            _context.Remove(note);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
