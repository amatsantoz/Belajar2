namespace Belajar2
{
    public class OutboxEvent
    {
        public int Id { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Processed { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
