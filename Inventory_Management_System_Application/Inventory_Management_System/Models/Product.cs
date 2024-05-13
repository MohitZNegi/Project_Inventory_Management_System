using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        // Foreign key referencing the Supplier entity
        [ForeignKey("SupplierKey")]
        public int SupplierID { get; set; }

        // Navigation property to represent the relationship

        public Supplier Supplier { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ProductName { get; set; }

        [MaxLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public float ProductPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ProductQuantity { get; set; }

        public string? ProductImg { get; set; }

 

        [Required]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        [Required]
        [MaxLength(100)]

        public string ProductSuppliers { get; set; }


        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; }


    }
}