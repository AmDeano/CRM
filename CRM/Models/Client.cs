using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
