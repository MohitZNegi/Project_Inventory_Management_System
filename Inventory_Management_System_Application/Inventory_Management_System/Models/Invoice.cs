using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Inventory_Management_System.Models
{

    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public string InvoiceFilePath { get; set; }
        public string PaymentStatus { get; set; }  // Pending or Completed
    }



}
