namespace GameHubStore.Models
{
    public class Wishlist
    {
        public int WishlistId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }
    }
}
