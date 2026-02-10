// Placeholder for DashboardDto
// This will contain Data Transfer Object for Dashboard statistics
namespace TaskManager.Business.DTOs
{
    public class DashboardDto
    {
        // Current Month Task Statistics
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int ToDoTasks { get; set; }
        public int OverdueTasksCount { get; set; }

        // Current Month Percentages
        public double CompletionPercentage { get; set; }
        public double InProgressPercentage { get; set; }
        public double OverduePercentage { get; set; }

        // All-Time Statistics
        public int AllTimeTotalTasks { get; set; }
        public int AllTimeCompletedTasks { get; set; }
        public double AllTimeCompletionPercentage { get; set; }

        // Priority Breakdown (Current Month)
        public int CriticalPriorityTasks { get; set; }
        public int HighPriorityTasks { get; set; }
        public int MediumPriorityTasks { get; set; }
        public int LowPriorityTasks { get; set; }

        // Upcoming Tasks
        public List<TaskDto> UpcomingTasks { get; set; } = new();
        public List<TaskDto> OverdueTasks { get; set; } = new();

        // Performance Score Data
        public Dictionary<string, double> WeeklyPerformanceScore { get; set; } = new();

        // Recommended Task
        public TaskDto? RecommendedTask { get; set; }
    }
}
