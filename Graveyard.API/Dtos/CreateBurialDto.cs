namespace Graveyard.API.Dtos;

// Tek adimda defin kaydi: kisi + vefat bilgileri birlikte
public record CreateBurialDto(
    string Ssn,
    string? FirstName,
    string? LastName,
    DateOnly? DateOfBirth,
    string? Gender,
    string? MotherName,
    string? FatherName,
    DateOnly? DateOfDeath,
    DateOnly? BurialDate,
    string? CauseOfDeath,
    string? Religion,
    string? VeteranStatus,
    string? FuneralPreferences,
    string? PlotNumber,
    string? PermitNumber
);
