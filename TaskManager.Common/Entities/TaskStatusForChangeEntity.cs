using TaskManager.Common.Enums;

namespace TaskManager.Common.Entities
{
	public class TaskStatusForChangeEntity
	{
		public int Id { get; set; }
		public required Guid TaskId { get; set; }
		public required TaskStatusesEnum StatusTo { get; set; }
		public required DateTime CreatedAt { get; set; }
		
		public TaskEntity Task { get; set; }
	}
}

