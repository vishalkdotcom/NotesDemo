using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesDemo.Entities
{
    [Table("AspNetRefreshTokens")]
    public class RefreshToken
    {
        public string Id { get; set; }

        public DateTime IssuedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }

        public string Token { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}