using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

        public class ProductView
        {


        [Required]
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        [Required]
        [Range(0.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float ProductPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int ProductQuantity { get; set; }

        [Required]
        public string Supplier { get; set; }
        public string CreatedBy { get; set; }

        public IFormFile ProductImg { get; set; }
    }
    }


