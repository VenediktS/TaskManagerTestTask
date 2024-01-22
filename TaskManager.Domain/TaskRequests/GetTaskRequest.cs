using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Common.DTOs.TaskDTOs;
using TaskManager.DB;

namespace TaskManager.Domain.TaskRequests
{
	public class GetTaskRequest : IRequest<GetTaskDTO?>
	{
		private readonly Guid _taskId;

		public GetTaskRequest(Guid taskId)
		{
			_taskId = taskId;
		}

        public class GetTaskRequestHandler : BaseTaskHandler, IRequestHandler<GetTaskRequest, GetTaskDTO?>
        {
            public GetTaskRequestHandler(TaskManagerDbContext dbContext, ILogger<GetTaskRequestHandler> logger) : base(dbContext, logger)
            {
            }

            public async Task<GetTaskDTO?> Handle(GetTaskRequest request, CancellationToken cancellationToken)
            {
                var entity = await _dbContext.Tasks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(el => el.Id == request._taskId, cancellationToken);

                if(entity is null)
                {
                    return null;
                }
                var result = new GetTaskDTO(Enum.GetName(entity.Status)!);

                return result;
            }
        }
    }
}

