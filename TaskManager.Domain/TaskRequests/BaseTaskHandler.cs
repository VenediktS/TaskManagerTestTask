using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Common.Entities;
using TaskManager.Common.Enums;
using TaskManager.DB;
using TaskManager.Domain.TaskDomain;

namespace TaskManager.Domain.TaskRequests
{
	public class BaseTaskHandler
	{
        protected readonly ILogger<BaseTaskHandler> _logger;
		protected readonly TaskManagerDbContext _dbContext;

        public BaseTaskHandler(TaskManagerDbContext dbContext, ILogger<BaseTaskHandler> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		protected async Task Add(TaskEntity entity, CancellationToken cancellationToken)
		{
            await _dbContext.Tasks.AddAsync(entity, cancellationToken);

			await CheckForNeedStatusChange(entity, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


		protected async Task Update(TaskEntity entity, CancellationToken cancellationToken) 
		{
             _dbContext.Tasks.Update(entity);

            await CheckForNeedStatusChange(entity, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task CheckForNeedStatusChange(TaskEntity entity, CancellationToken cancellationToken)
		{
			var nextStatus = TaskStatusUpdateRulesService.GetNextStatus(entity.Status);
			if (nextStatus is null)
			{
				return;
			}

			var existChange = await _dbContext.TaskStatusForChangeEntities.AnyAsync(el => el.TaskId == entity.Id && el.StatusTo == nextStatus);
			if (existChange)
			{
				return;
			}

			var newTaskStatusChange = new TaskStatusForChangeEntity()
			{
				TaskId = entity.Id,
				StatusTo = (TaskStatusesEnum)nextStatus,
				CreatedAt = DateTime.Now,
			};

			_dbContext.TaskStatusForChangeEntities.Add(newTaskStatusChange);
        }
	}
}

