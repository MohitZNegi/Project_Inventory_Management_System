using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Inventory_Management_System.Models
{
    // Junction table for the many-to-many relationship between products and suppliers
    public class ProductSupplier
    {
        [Key]

        public int ProductSupplierId { get; set; }


        // Foreign key properties

        public int ProductID { get; set; }

        public Product? Product { get; set; }

        public int SupplierID { get; set; }
        public Supplier? Supplier { get; set; }


        public Product Product { get; set; }

        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; }

    }
}

