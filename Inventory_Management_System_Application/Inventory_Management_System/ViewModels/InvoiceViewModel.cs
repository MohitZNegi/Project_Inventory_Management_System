using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

    

        public class InvoiceViewModel
        {
            public int InvoiceId { get; set; }
            public int OrderId { get; set; }
            public string UserName { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Status { get; set; }
            public float TotalAmount { get; set; }
            public string InvoiceFilePath { get; set; } // Path to the PDF invoice
            public string PaymentStatus { get; set; }  // Pending or Completed
        }
    
    }


