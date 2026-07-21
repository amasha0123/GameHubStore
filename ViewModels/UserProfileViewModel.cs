using GameHubStore.Models;
using System.Collections.Generic;

namespace GameHubStore.ViewModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser? User { get; set; }
        public List<Order>? Orders { get; set; }
        public List<Wishlist>? Wishlists { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
