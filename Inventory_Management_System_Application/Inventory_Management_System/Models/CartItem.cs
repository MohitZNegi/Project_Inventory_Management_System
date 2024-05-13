using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Inventory_Management_System.Models
{
  
        public class CartItem
        {
            [Key]
            public int CartID { get; set; }
            [ForeignKey("User")]
            public string UserId { get; set; }

            public ApplicationUser User { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
        }


  
}
