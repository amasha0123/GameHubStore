using System.Collections.Generic;

namespace GameHubStore.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Game>? Games { get; set; }
    }
}
