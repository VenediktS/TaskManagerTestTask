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
					StatusSetAt = DateTime.Now,
					UpdateAt = DateTime.Now,
					CreatedAt = DateTime.Now
				};

				await Add(entity, cancellationToken);

				return entity.Id;
            }
        }
	}
}
