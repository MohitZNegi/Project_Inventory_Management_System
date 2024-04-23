using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

        public class ClientView
        {
         

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }
    }


