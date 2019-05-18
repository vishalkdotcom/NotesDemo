using System.ComponentModel.DataAnnotations.Schema;

namespace NotesDemo.Entities
{
    public class Note
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsPinned { get; set; }

        public int CreatedAt { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}