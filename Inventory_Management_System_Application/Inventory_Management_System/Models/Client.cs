using Inventory_Management_System.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Inventory_Management_System.Models
{
    public class Client
    {
        [Key]
        [Required]
        public string Client_Id { get; set; }

        public virtual ApplicationUser UserData { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string First_name { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string Last_name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }



    }
}
