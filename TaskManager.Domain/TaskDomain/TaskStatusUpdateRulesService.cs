using System.Linq;
using System.Net.NetworkInformation;
using TaskManager.Common.Entities;
using TaskManager.Common.Enums;

namespace TaskManager.Domain.TaskDomain
{
    public static class TaskStatusUpdateRulesService
    {
        public static bool IsInAnUpdatedStatus(TaskEntity taskEntity)
        {
            return taskEntity.Status switch
            {
                TaskStatusesEnum.Created => true,
                TaskStatusesEnum.Running => true,
                _ => false
            };
        }

        public static bool CanUpdate(TaskEntity taskEntity)
        {
            return taskEntity.Status switch
            {
                TaskStatusesEnum.Created => true,
                TaskStatusesEnum.Running => OnRunningRule(taskEntity),
                TaskStatusesEnum.Finished => false,
                _ => false
            };
        }

        public static TaskStatusesEnum? GetNextStatus(TaskStatusesEnum currentStatus)
        {
            return currentStatus switch
            {
                TaskStatusesEnum.Created => TaskStatusesEnum.Running,
                TaskStatusesEnum.Running => TaskStatusesEnum.Finished,
                _ => null
            };
        }

        private static bool OnRunningRule(TaskEntity entity)
		{
            if (DateTimeOffset.Now >= entity.StatusSetAt.AddMinutes(2))
            {
                return true;
            }
            return false;
        } 
	}
}

