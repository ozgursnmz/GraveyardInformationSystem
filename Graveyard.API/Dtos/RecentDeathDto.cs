namespace Graveyard.API.Dtos;

// Halka acik vefat ilani (yalnizca ad + konum + tarih)
public record RecentDeathDto(
    string? Name,
    DateOnly? DateOfDeath,
    string? ZoneName,
    string? PlotNumber
);
