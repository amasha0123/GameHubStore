using System;

namespace GameHubStore.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
