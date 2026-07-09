namespace Graveyard.API.Dtos;

// Dashboard istatistik kartlari icin ozet veri
public record StatsDto(
    int TotalPlots,
    int OccupiedPlots,
    int AvailablePlots,
    int ReservedPlots,
    double OccupancyRate,
    int TotalDeceased,
    double TotalRevenue,    // toplam gelir (odemeler)
    double TotalExpense,    // toplam gider (bakim maliyetleri)
    double NetProfit        // net kar = gelir - gider
);
