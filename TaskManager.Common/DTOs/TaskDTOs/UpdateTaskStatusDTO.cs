using TaskManager.Common.Enums;

namespace TaskManager.Common.DTOs.TaskDTOs
{
	public record UpdateTaskStatusDTO(Guid Id, TaskStatusesEnum Status);
}

