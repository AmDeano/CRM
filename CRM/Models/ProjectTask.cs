using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }

        public bool IsComplted  { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Urgent
    }
}
