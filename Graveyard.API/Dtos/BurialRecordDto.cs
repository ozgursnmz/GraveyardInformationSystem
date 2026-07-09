namespace Graveyard.API.Dtos;

// Vefat eden -> Mezar yeri -> Bolge -> Anit -> Sahip zincirini
// tek anlamli nesnede birlestiren okuma DTO'su.
public record BurialRecordDto(
    string Ssn,
    string? DeceasedName,
    DateOnly? DateOfBirth,
    DateOnly? DateOfDeath,
    string? CauseOfDeath,
    string? Religion,
    string? PlotNumber,
    string? PlotStatus,
    string? ZoneName,
    string? Monument,
    string? OwnerName,
    string? OwnerPhone,
    string? OwnerEmail,
    string? PermitNumber
);
