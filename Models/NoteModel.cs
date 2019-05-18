using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NotesDemo.Models
{
    public class NoteModel
    {
        public string Id { get; internal set; }

        [Required]
        [StringLength(64)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Content { get; set; }

        [Required]
        public bool IsPinned { get; set; }

        public int CreatedAt { get; internal set; }
    }
}