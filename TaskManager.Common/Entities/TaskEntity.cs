using TaskManager.Common.Enums;

namespace TaskManager.Common.Entities
{
	public class TaskEntity
	{
		public Guid Id { get; set; }
		public required TaskStatusesEnum Status { get; set; }
		public DateTimeOffset StatusSetAt { get; set; }

		public DateTimeOffset UpdateAt { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
    }
}

