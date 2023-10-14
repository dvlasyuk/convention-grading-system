namespace EventGradingSystem.Models.ParticipantGrade
{
    public class GradeType
    {
        public int Identifier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimalGrage { get; set; }
        public int MaximalGrage { get; set; }
    }
}
