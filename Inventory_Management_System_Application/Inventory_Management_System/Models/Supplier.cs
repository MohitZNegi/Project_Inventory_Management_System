using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Inventory_Management_System.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }



        [Required]
        [MaxLength(100)]
        public string? SupplierName { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        [MaxLength(100)]
        public string? ContactDetails { get; set; }

        [NotMapped]
        public string? Products { get; set; }

        [Required]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation property for products supplied by this supplier
        public List<ProductSupplier>? ProductSuppliers { get; set; }
    }
}