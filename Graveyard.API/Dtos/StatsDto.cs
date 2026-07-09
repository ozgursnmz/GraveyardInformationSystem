namespace Graveyard.API.Dtos;

// Dashboard istatistik kartlari icin ozet veri
public record StatsDto(
    int TotalPlots,
    int OccupiedPlots,
    int AvailablePlots,
    int ReservedPlots,
    double OccupancyRate,
    int TotalDeceased,
    double TotalRevenue
);
