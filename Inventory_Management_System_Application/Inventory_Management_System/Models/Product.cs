using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Inventory_Management_System.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [MaxLength(500)]
        public string ProductDescription { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public float ProductPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ProductQuantity { get; set; }

        public string? ProductImg { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; }

        [Required]
        [MaxLength(100)]
        public string Supplier { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; }

    }
}
