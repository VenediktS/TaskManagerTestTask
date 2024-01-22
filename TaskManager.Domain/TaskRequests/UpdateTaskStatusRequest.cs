using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Common.DTOs.TaskDTOs;
using TaskManager.DB;
using TaskManager.Domain.TaskDomain;

namespace TaskManager.Domain.TaskRequests
{
	public class UpdateTaskStatusRequest : IRequest 
	{
		private readonly UpdateTaskStatusDTO _model;
        public UpdateTaskStatusRequest(UpdateTaskStatusDTO model)
		{
			_model = model;
		}

        public class UpdateTaskStatusRequestHandler : BaseTaskHandler, IRequestHandler<UpdateTaskStatusRequest>
        {

            public UpdateTaskStatusRequestHandler(TaskManagerDbContext dbContext, ILogger<UpdateTaskStatusRequestHandler> logger): base(dbContext, logger)
            {
            }

            public async Task Handle(UpdateTaskStatusRequest request, CancellationToken cancellationToken)
            {
               var entity = await _dbContext.Tasks.FirstOrDefaultAsync(el =>  el.Id == request._model.Id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogCritical($"Task entity with id: {request._model.Id} - not found, when it try to update status - {Enum.GetName(request._model.Status)}");
                    return;
                }

                entity.Status = request._model.Status;
                entity.StatusSetAt = DateTimeOffset.Now;
                entity.UpdateAt = DateTimeOffset.Now;

                _dbContext.Update(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

