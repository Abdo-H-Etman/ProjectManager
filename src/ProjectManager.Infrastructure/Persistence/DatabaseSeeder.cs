using Domain.Entities;
using Domain.Enums;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DomainTask = Domain.Entities.Task;

namespace Infrastructure.Persistence;

public class DatabaseSeeder
{
    private const string SeedEmail = "demo@projectmanager.local";
    private const string SeedPassword = "Password123";

    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public DatabaseSeeder(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async System.Threading.Tasks.Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        var user = await _userManager.FindByEmailAsync(SeedEmail);
        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = SeedEmail,
                Email = SeedEmail,
                EmailConfirmed = true,
                FirstName = "Demo",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, SeedPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create the seed user: {errors}");
            }
        }

        var projectCount = await _dbContext.Projects
            .CountAsync(project => project.OwnerId == user.Id, cancellationToken);
        if (projectCount > 0)
        {
            return;
        }

        var utcToday = DateTime.UtcNow.Date;
        var websiteProject = new Project
        {
            Name = "Website Redesign",
            Description = "Refresh the public website and improve the onboarding experience.",
            Status = ProjectStatus.Active,
            OwnerId = user.Id,
            StartDate = utcToday.AddDays(-14),
            EndDate = utcToday.AddDays(30)
        };
        var mobileProject = new Project
        {
            Name = "Mobile App Planning",
            Description = "Prepare the product and technical plan for the mobile application.",
            Status = ProjectStatus.OnHold,
            OwnerId = user.Id,
            StartDate = utcToday.AddDays(-3),
            EndDate = utcToday.AddDays(45)
        };

        var designTask = new DomainTask
        {
            Project = websiteProject,
            Title = "Create the new landing page",
            Description = "Prepare the first responsive landing-page implementation.",
            Priority = TaskPriority.High,
            Status = Domain.Enums.TaskStatus.InProgress,
            StartDate = utcToday.AddDays(-5),
            DueDate = utcToday.AddDays(7),
            AssignedToId = user.Id,
            AssignedAt = DateTime.UtcNow,
            CreatedById = user.Id,
            EstimatedHours = 16
        };
        var reviewTask = new DomainTask
        {
            Project = websiteProject,
            Title = "Review accessibility checklist",
            Description = "Check keyboard navigation, contrast, and semantic markup.",
            Priority = TaskPriority.Medium,
            Status = Domain.Enums.TaskStatus.Pending,
            DueDate = utcToday.AddDays(12),
            AssignedToId = user.Id,
            CreatedById = user.Id,
            EstimatedHours = 6
        };
        var planningTask = new DomainTask
        {
            Project = mobileProject,
            Title = "Draft mobile app requirements",
            Description = "Collect the initial feature and platform requirements.",
            Priority = TaskPriority.Critical,
            Status = Domain.Enums.TaskStatus.Blocked,
            DueDate = utcToday.AddDays(20),
            CreatedById = user.Id,
            EstimatedHours = 10
        };

        var initialComment = new Comment
        {
            Task = designTask,
            AuthorId = user.Id,
            Content = "The first design pass is ready for review.",
            EditedAt = DateTime.UtcNow
        };
        var replyComment = new Comment
        {
            Task = designTask,
            ParentComment = initialComment,
            AuthorId = user.Id,
            Content = "I will review it and add notes today.",
            EditedAt = DateTime.UtcNow
        };

        _dbContext.Projects.AddRange(websiteProject, mobileProject);
        _dbContext.Tasks.AddRange(designTask, reviewTask, planningTask);
        _dbContext.Comments.AddRange(initialComment, replyComment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}