using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace CRM.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(128)]
        public string FirstName { get; set; }
        [MaxLength(128)]
        public string LastName { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
