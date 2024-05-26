using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}


