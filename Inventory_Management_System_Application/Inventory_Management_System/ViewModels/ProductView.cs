using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

    public class ProductView
    {
        public ProductView()
        {
            Suppliers = new List<SelectListItem>(); // Initialize Suppliers list
        }

        public string? ProductName { get; set; }

        public string? ProductDescription { get; set; }

        [Required]
        [Range(0.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float ProductPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int ProductQuantity { get; set; }


        public string? Supplier { get; set; }

        [Required]

        public string? CreatedBy { get; set; }
        public string? ProductSuppliers { get; set; }

        public IFormFile? ProductImg { get; set; }
        public List<SelectListItem> Suppliers { get; set; }
    }
}