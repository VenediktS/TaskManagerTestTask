using Microsoft.Extensions.Hosting;
using TaskManager.Domain.TaskDomain;
using TaskManager.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using TaskManager.Domain.TaskRequests;
using TaskManager.Common.DTOs.TaskDTOs;
using TaskManager.Common.Entities;

namespace TaskManager.Domain.Jobs
{
    public class TaskStatusUpdateJob : IHostedService
    {
        private readonly TaskManagerDbContext _context;
        private readonly ILogger<TaskStatusUpdateJob> _logger;
        private readonly IMediator _mediator;

        public TaskStatusUpdateJob(
            TaskManagerDbContext context,
            ILogger<TaskStatusUpdateJob> logger,
            IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task UpdateStatuses(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var statusForChange = _context.TaskStatusForChangeEntities
                        .AsNoTracking()
                        .OrderByDescending(el => el.CreatedAt)
                        .Include(el => el.Task)
                        .AsAsyncEnumerable();

                var changesToRemove = new List<TaskStatusForChangeEntity>();

                await foreach (var change in statusForChange)
                {
                    if (TaskStatusUpdateRulesService.IsInAnUpdatedStatus(change.Task))
                    {
                        _logger.LogWarning(
                            $"Task with id: {change.Task} add to queue for status update but, has status: {Enum.GetName(change.Task.Status)} and cant be changed");
                        changesToRemove.Add(change);
                        continue;
                    }

                    if (TaskStatusUpdateRulesService.CanUpdate(change.Task))
                    {
                        await _mediator.Send(new UpdateTaskStatusRequest(new UpdateTaskStatusDTO(change.Task.Id, change.Task.Status)), cancellationToken);
                        changesToRemove.Add(change);
                    }
                }

                _context.TaskStatusForChangeEntities.RemoveRange(changesToRemove);
                _context.SaveChanges();

                await Task.Delay(1000, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

