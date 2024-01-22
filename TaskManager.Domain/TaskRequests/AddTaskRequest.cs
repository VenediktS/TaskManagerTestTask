using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Common.Entities;
using TaskManager.Common.Enums;
using TaskManager.DB;

namespace TaskManager.Domain.TaskRequests
{
	public class AddTaskRequest : IRequest<Guid>
	{
		public AddTaskRequest()
		{
		}

		public class AddTaskRequestHandler : BaseTaskHandler, IRequestHandler<AddTaskRequest, Guid>
		{
			public AddTaskRequestHandler(TaskManagerDbContext dbContext, ILogger<AddTaskRequestHandler> logger): base(dbContext, logger)
			{
			}

            public async Task<Guid> Handle(AddTaskRequest request, CancellationToken cancellationToken)
            {
				var entity = new TaskEntity()
				{
					Status = TaskStatusesEnum.Created,
					StatusSetAt = DateTimeOffset.Now,
					UpdateAt = DateTimeOffset.Now,
					CreatedAt = DateTimeOffset.Now
				};

				await _dbContext.Tasks.AddAsync(entity, cancellationToken);
				await _dbContext.SaveChangesAsync(cancellationToken);

				return entity.Id;
            }
        }
	}
}
