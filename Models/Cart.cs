namespace GameHubStore.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }
        public int Quantity { get; set; }
    }
}
