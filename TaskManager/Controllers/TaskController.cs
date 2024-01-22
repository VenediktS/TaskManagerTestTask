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
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddTask(CancellationToken cancellationToken)
		{
			var entityId = await _mediator.Send(new AddTaskRequest(), cancellationToken);

			return Accepted(entityId);
		}

		[HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

