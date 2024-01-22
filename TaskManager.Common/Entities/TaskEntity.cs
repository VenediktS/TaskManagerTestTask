using TaskManager.Common.Enums;

namespace TaskManager.Common.Entities
{
	public class TaskEntity
	{
		public Guid Id { get; set; }
		public required TaskStatusesEnum Status { get; set; }
		public DateTime StatusSetAt { get; set; }

		public DateTime UpdateAt { get; set; }
		public DateTime CreatedAt { get; set; }
    }
}

