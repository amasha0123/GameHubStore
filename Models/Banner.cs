namespace GameHubStore.Models
{
    public class Banner
    {
        public int BannerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string LinkUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
