namespace ConventionGradingSystem.Database.Entities
{
    public class ParticipantGrade
    {
        public int Identifier { get; set; }
        public int EventTypeId { get; set; }
        public int EventId { get; set; }
        public int GradeTypeId { get; set; }
        public int GradeValue { get; set; }
    }
}
