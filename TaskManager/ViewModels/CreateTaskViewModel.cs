using TaskStatus = TaskManager.DataAccess.Enums.TaskStatus;
using System.ComponentModel.DataAnnotations;
using TaskManager.DataAccess.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Validation;

namespace TaskManager.ViewModels
{
    public class CreateTaskViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Task Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Deadline is required")]
        [FutureDate(allowToday: true)]
        [Display(Name = "Deadline")]
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // For dropdown
        public IEnumerable<SelectListItem>? Categories { get; set; }

        // Helper property for formatted datetime (without seconds)
        public string DeadlineFormatted => Deadline.ToString("yyyy-MM-ddTHH:mm");
        
        // Helper property for HTML5 min attribute (today's date)
        public string MinDeadline => DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
    }
}
