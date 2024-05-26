using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime OrderCreatedAt { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderPlacedBy { get; set; }
        public string ShippingAddress { get; set; }
        public string OrderStatus { get; set; }
        public float TotalAmount { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}


