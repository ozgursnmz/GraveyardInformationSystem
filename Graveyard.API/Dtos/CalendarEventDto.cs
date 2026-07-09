namespace Graveyard.API.Dtos;

// Cenaze takvimi olayi
public record CalendarEventDto(
    string ServiceId,
    string? DeceasedName,
    string? ServiceType,
    DateOnly? ServiceDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    int? ExpectedAttendees
);
