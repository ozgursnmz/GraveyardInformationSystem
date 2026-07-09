namespace Graveyard.API.Dtos;

// Harita uzerinde bir parseli temsil eder
public record MapPlotDto(
    string PlotNumber,
    double? Latitude,
    double? Longitude,
    string? Status,
    string? ZoneName,
    string? Occupant,   // yatan kisi (varsa)
    int? DeathYear      // olum yili (sorgulama filtresi icin)
);
