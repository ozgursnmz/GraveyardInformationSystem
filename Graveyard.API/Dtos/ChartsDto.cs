namespace Graveyard.API.Dtos;

// Grafik verileri icin basit etiket-deger cifti
public record LabelValue(string Label, double Value);

public record ChartsDto(
    List<LabelValue> ZoneOccupancy,      // bolgelere gore doluluk
    List<LabelValue> DeathsByMonth,      // aylara gore vefat sayisi
    List<LabelValue> PaymentMethods,     // odeme yontemi dagilimi
    double Income,                       // toplam gelir
    double Expense,                      // toplam gider
    List<LabelValue> MaintenanceByMonth  // aylara gore bakim maliyeti
);
