using System;
using System.Collections.Generic;

namespace GameHubStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentMethod { get; set; } = string.Empty;

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
