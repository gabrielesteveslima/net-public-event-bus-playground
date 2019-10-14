using System;

namespace BariBank.EventBus.Events
{
    public abstract class Event
    {
        public Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Event(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }

        public string ServiceId { get; set; }

        public DateTime CreationDate { get; }
        public Guid Id { get; set; }
    }
}
