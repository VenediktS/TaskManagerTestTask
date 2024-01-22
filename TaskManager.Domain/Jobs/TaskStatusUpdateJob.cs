using Microsoft.Extensions.Hosting;
using TaskManager.Domain.TaskDomain;
using TaskManager.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using TaskManager.Domain.TaskRequests;
using TaskManager.Common.DTOs.TaskDTOs;
using TaskManager.Common.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Domain.Jobs
{
    public class TaskStatusUpdateJob : IHostedService
    {
        private readonly ILogger<TaskStatusUpdateJob> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TaskStatusUpdateJob(
            ILogger<TaskStatusUpdateJob> logger,
            IMediator mediator,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            UpdateStatuses(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task UpdateStatuses(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var context = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

                    var statusForChange = await context.TaskStatusForChangeEntities
                        .OrderByDescending(el => el.CreatedAt)
                        .Include(el => el.Task)
                        .ToListAsync();

                        var changesToRemove = new List<TaskStatusForChangeEntity>();

                        foreach (var change in statusForChange)
                        {
                            if (!TaskStatusUpdateRulesService.IsInAnUpdatedStatus(change.Task))
                            {
                                _logger.LogWarning(
                                    $"Task with id: {change.Task} add to queue for status update but, has status: {Enum.GetName(change.Task.Status)} and cant be changed");
                                changesToRemove.Add(change);
                                continue;
                            }

                            if (TaskStatusUpdateRulesService.CanUpdate(change.Task))
                            {
                                await mediator.Send(new UpdateTaskStatusRequest(new UpdateTaskStatusDTO(change.TaskId, change.StatusTo)), cancellationToken);
                                changesToRemove.Add(change);
                            }
                        }

                        context.TaskStatusForChangeEntities.RemoveRange(changesToRemove);
                        context.SaveChanges();
                    }

                    await Task.Delay(1000, cancellationToken);
                }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

