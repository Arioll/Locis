using Common.Other;

namespace Common.Entities.AbstractEntities
{
    public abstract class Notification
    {
        public virtual NotificationTypes Type { get; protected set; }
    }
}
