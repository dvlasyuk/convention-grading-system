namespace EventGradingSystem.Database.Entities
{
    public class ExpertNote
    {
        public int Identifier { get; set; }
        public int EventTypeId { get; set; }
        public int EventId { get; set; }
        public string Note { get; set; }
    }
}
