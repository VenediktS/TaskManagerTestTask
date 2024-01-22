using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Common.DTOs.TaskDTOs;
using TaskManager.Domain.TaskRequests;

namespace TaskManagerWeb.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TaskController : ControllerBase
	{
		private readonly IMediator _mediator;

		public TaskController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> AddTask(CancellationToken cancellationToken)
		{
			var entityId = await _mediator.Send(new AddTaskRequest(), cancellationToken);

			return Accepted(entityId);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<GetTaskDTO>> GetTask([FromRoute]Guid id, CancellationToken cancellationToken)
		{
			var entity = await _mediator.Send(new GetTaskRequest(id), cancellationToken);

			if (entity is null)
			{
                return NotFound();
            }

			return Ok(entity);
        }
	}
}

