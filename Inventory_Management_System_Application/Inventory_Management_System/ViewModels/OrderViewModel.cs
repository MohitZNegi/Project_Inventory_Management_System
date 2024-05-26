using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

    public class OrderViewModel
    {
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string ShippingAddress { get; set; }
    }
}


