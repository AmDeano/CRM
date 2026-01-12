using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ProjectStatus Status { get; set; }

        public int ClientId { get; set; }

        public virtual Client Client { get; set; }

        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        public double Progress
        {
            get
            {
                if (Tasks == null || !Tasks.Any()) 
                    return 0;
                return Math.Round(
                    (double)Tasks.Count(t => t.IsCompleted) / Tasks.Count * 100, 2);
            }
        }
    }

    public enum ProjectStatus
    {
        NotStarted,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }
}
