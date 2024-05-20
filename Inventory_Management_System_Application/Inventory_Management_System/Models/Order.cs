using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Inventory_Management_System.Models
{

    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        [Required]
        [DataType(DataType.DateTime)]
        public DateTime OrderCreatedAt  { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderPlacedBy { get; set; }
        public string ShippingAddress { get; set; }

        public string OrderStatus { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public float TotalAmount { get; set; }

        
    }


}
