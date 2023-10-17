namespace ConventionGradingSystem.Models.MainPage;

public record Team(
    string Name,
    int MembersQuantity,
    int MembersRegistrationsQuantity,
    int MembersMarksQuantity,
    int SpecialMarksQuantity);
