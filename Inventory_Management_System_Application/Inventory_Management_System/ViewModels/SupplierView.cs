using Inventory_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

        public class SupplierView
        {


       

        [Required]
        [MaxLength(100)]
        public string? SupplierName { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        [MaxLength(100)]
        public string? ContactDetails { get; set; }


        public string? Products { get; set; }


        // Additional property for products
        public List<ProductView> Products { get; set; }

        [Required]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
    }


