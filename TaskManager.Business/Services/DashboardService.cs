using TaskStatus = TaskManager.DataAccess.Enums.TaskStatus;
using AutoMapper;
using TaskManager.Business.DTOs;
using TaskManager.DataAccess.Enums;
using TaskManager.DataAccess.Repositories;

namespace TaskManager.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRecommendationService _recommendationService;
        private readonly IMapper _mapper;

        public DashboardService(
            ITaskRepository taskRepository,
            ICategoryRepository categoryRepository,
            IRecommendationService recommendationService,
            IMapper mapper)
        {
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _recommendationService = recommendationService;
            _mapper = mapper;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(string userId)
        {
            var allTasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            var tasksList = allTasks.ToList();

            // Get current month and year
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            // Filter tasks for current month (based on creation date)
            var currentMonthTasks = tasksList
                .Where(t => t.CreatedAt.Month == currentMonth && t.CreatedAt.Year == currentYear)
                .ToList();

            var dashboard = new DashboardDto
            {
                // Current Month Task counts by status
                TotalTasks = currentMonthTasks.Count,
                CompletedTasks = currentMonthTasks.Count(t => t.Status == TaskStatus.Completed),
                InProgressTasks = currentMonthTasks.Count(t => t.Status == TaskStatus.InProgress),
                ToDoTasks = currentMonthTasks.Count(t => t.Status == TaskStatus.ToDo),
                OverdueTasksCount = currentMonthTasks.Count(t => t.Deadline < DateTime.UtcNow && t.Status != TaskStatus.Completed),

                // Current Month Task counts by priority
                CriticalPriorityTasks = currentMonthTasks.Count(t => t.Priority == TaskPriority.Critical),
                HighPriorityTasks = currentMonthTasks.Count(t => t.Priority == TaskPriority.High),
                MediumPriorityTasks = currentMonthTasks.Count(t => t.Priority == TaskPriority.Medium),
                LowPriorityTasks = currentMonthTasks.Count(t => t.Priority == TaskPriority.Low),

                // All-Time Statistics
                AllTimeTotalTasks = tasksList.Count,
                AllTimeCompletedTasks = tasksList.Count(t => t.Status == TaskStatus.Completed)
            };

            // Calculate current month percentages
            if (dashboard.TotalTasks > 0)
            {
                dashboard.CompletionPercentage = Math.Round((double)dashboard.CompletedTasks / dashboard.TotalTasks * 100, 2);
                dashboard.InProgressPercentage = Math.Round((double)dashboard.InProgressTasks / dashboard.TotalTasks * 100, 2);
                dashboard.OverduePercentage = Math.Round((double)dashboard.OverdueTasksCount / dashboard.TotalTasks * 100, 2);
            }

            // Calculate all-time completion percentage
            if (dashboard.AllTimeTotalTasks > 0)
            {
                dashboard.AllTimeCompletionPercentage = Math.Round((double)dashboard.AllTimeCompletedTasks / dashboard.AllTimeTotalTasks * 100, 2);
            }

            // Get upcoming and overdue tasks (use all tasks, not just current month)
            var upcomingTasks = await _taskRepository.GetUpcomingTasksAsync(userId, 7);
            dashboard.UpcomingTasks = _mapper.Map<List<TaskDto>>(upcomingTasks.Take(10));

            var overdueTasks = await _taskRepository.GetOverdueTasksAsync(userId);
            dashboard.OverdueTasks = _mapper.Map<List<TaskDto>>(overdueTasks.Take(10));

            // Calculate weekly performance score (priority-weighted)
            dashboard.WeeklyPerformanceScore = CalculateWeeklyPerformanceScore(tasksList);

            // Get recommended task (use all incomplete tasks)
            var recommendedTask = await _recommendationService.GetRecommendedTaskAsync(userId);
            dashboard.RecommendedTask = recommendedTask?.Task;

            return dashboard;
        }

        private Dictionary<string, double> CalculateWeeklyPerformanceScore(List<DataAccess.Models.UserTask> tasks)
        {
            var performanceScores = new Dictionary<string, double>();

            // Priority weights for scoring
            var priorityWeights = new Dictionary<TaskPriority, double>
            {
                { TaskPriority.Critical, 4.0 },
                { TaskPriority.High, 3.0 },
                { TaskPriority.Medium, 2.0 },
                { TaskPriority.Low, 1.0 }
            };

            // Get last 7 days
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                var dayName = date.ToString("ddd"); // Mon, Tue, Wed, etc.

                // Calculate performance score for this day
                var completedTasksToday = tasks.Where(t => 
                    t.CompletedAt.HasValue && 
                    t.CompletedAt.Value.Date == date).ToList();

                double dailyScore = 0;
                foreach (var task in completedTasksToday)
                {
                    // Weight by priority
                    var weight = priorityWeights.ContainsKey(task.Priority) 
                        ? priorityWeights[task.Priority] 
                        : 1.0;
                    
                    dailyScore += weight;
                }

                performanceScores[dayName] = dailyScore;
            }

            return performanceScores;
        }

        public async Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(
            string userId,
            int? categoryId = null,
            TaskStatus? status = null,
            TaskPriority? priority = null)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);

            // Apply filters
            if (categoryId.HasValue)
                tasks = tasks.Where(t => t.CategoryId == categoryId.Value);

            if (status.HasValue)
                tasks = tasks.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                tasks = tasks.Where(t => t.Priority == priority.Value);

            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<IEnumerable<TaskDto>> GetSortedTasksAsync(string userId, string sortBy)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            var tasksList = tasks.ToList();

            var sortedTasks = sortBy.ToLower() switch
            {
                "urgency" => tasksList.OrderBy(t => t.Deadline).ThenByDescending(t => t.Priority),
                "priority" => tasksList.OrderByDescending(t => t.Priority).ThenBy(t => t.Deadline),
                "deadline" => tasksList.OrderBy(t => t.Deadline),
                "title" => tasksList.OrderBy(t => t.Title),
                "status" => tasksList.OrderBy(t => t.Status),
                "category" => tasksList.OrderBy(t => t.Category.Name),
                _ => tasksList.OrderBy(t => t.CreatedAt)
            };

            return _mapper.Map<IEnumerable<TaskDto>>(sortedTasks);
        }
    }
}
