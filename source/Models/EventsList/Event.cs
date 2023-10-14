using System.Collections.Generic;

namespace ConventionGradingSystem.Models.EventsList
{
    public class Event
    {
        public int Identifier { get; set; }
        public string Name { get; set; }
        public int ExprertGradesQuantity { get; set; }
        public int ParticipantGradesQuantity { get; set; }
        public Dictionary<int, float> ExpertGrades { get; set; }
        public Dictionary<int, float> ParticipantGrades { get; set; }
        public float TotalExpertGrade { get; set; }
        public float TotalParticipantGrade { get; set; }
        public float TotalGrade { get; set; }
        public bool WithParticipants { get; set; }
    }
}
