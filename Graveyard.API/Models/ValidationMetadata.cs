using System.ComponentModel.DataAnnotations;
using Graveyard.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

// Model dogrulama kurallari (DataAnnotations). Scaffold edilen model dosyalarini
// kirletmemek icin buddy (metadata) sinif deseni kullanildi: [ModelMetadataType].
// [ApiController] sayesinde kurallar POST/PUT'ta otomatik calisir; hatali istek 400 doner.
namespace Graveyard.API.Models;

// ------- Kimlik (TC) yardimcisi: 11 haneli rakam -------
// Person / GraveOwner / DeceasedPerson / Employee SSN alanlarinda kullanilir.

[ModelMetadataType(typeof(PersonMetadata))]
public partial class Person { }
public class PersonMetadata
{
    [Required(ErrorMessage = "TC Kimlik No zorunludur.")]
    [RegularExpression("^[0-9]{11}$", ErrorMessage = "TC Kimlik No 11 haneli rakam olmalıdır.")]
    public string Ssn { get; set; } = null!;

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    public string? LastName { get; set; }

    [NotFutureDate(ErrorMessage = "Doğum tarihi gelecekte olamaz.")]
    public DateOnly? DateOfBirth { get; set; }

    [StringLength(50)] public string? MotherName { get; set; }
    [StringLength(50)] public string? FatherName { get; set; }
}

[ModelMetadataType(typeof(GravePlotMetadata))]
public partial class GravePlot { }
public class GravePlotMetadata
{
    [Required(ErrorMessage = "Parsel No zorunludur.")]
    [StringLength(20, ErrorMessage = "Parsel No en fazla 20 karakter olabilir.")]
    public string PlotNumber { get; set; } = null!;

    [Range(0.1, 100, ErrorMessage = "Uzunluk 0.1 ile 100 m arasında olmalıdır.")]
    public double? Length { get; set; }

    [Range(0.1, 100, ErrorMessage = "Genişlik 0.1 ile 100 m arasında olmalıdır.")]
    public double? Width { get; set; }

    [Range(-90, 90, ErrorMessage = "Enlem -90 ile 90 arasında olmalıdır.")]
    public double? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Boylam -180 ile 180 arasında olmalıdır.")]
    public double? Longitude { get; set; }
}

[ModelMetadataType(typeof(CemeteryZoneMetadata))]
public partial class CemeteryZone { }
public class CemeteryZoneMetadata
{
    [Required(ErrorMessage = "Bölge kodu zorunludur.")]
    [StringLength(20)] public string ZoneId { get; set; } = null!;

    [Required(ErrorMessage = "Bölge adı zorunludur.")]
    [StringLength(100)] public string? Name { get; set; }

    [Range(0, 100000, ErrorMessage = "Kapasite 0 veya daha büyük olmalıdır.")]
    public int? TotalCapacity { get; set; }

    [Range(0, 100000, ErrorMessage = "Doluluk 0 veya daha büyük olmalıdır.")]
    public int? CurrentOccupancy { get; set; }
}

[ModelMetadataType(typeof(DeceasedPersonMetadata))]
public partial class DeceasedPerson { }
public class DeceasedPersonMetadata
{
    [Required(ErrorMessage = "TC Kimlik No zorunludur.")]
    [RegularExpression("^[0-9]{11}$", ErrorMessage = "TC Kimlik No 11 haneli rakam olmalıdır.")]
    public string Ssn { get; set; } = null!;

    [NotFutureDate(ErrorMessage = "Ölüm tarihi gelecekte olamaz.")]
    public DateOnly? DateOfDeath { get; set; }
}

[ModelMetadataType(typeof(PaymentMetadata))]
public partial class Payment { }
public class PaymentMetadata
{
    [Required(ErrorMessage = "Makbuz No zorunludur.")]
    [StringLength(30)] public string ReceiptNo { get; set; } = null!;

    [Range(0, 10_000_000, ErrorMessage = "Tutar negatif olamaz.")]
    public double? Amount { get; set; }

    [NotFutureDate(ErrorMessage = "Ödeme tarihi gelecekte olamaz.")]
    public DateOnly? PaymentDate { get; set; }

    [StringLength(3, ErrorMessage = "Para birimi en fazla 3 karakter (örn. TRY).")]
    public string? Currency { get; set; }
}

[ModelMetadataType(typeof(GraveOwnerMetadata))]
public partial class GraveOwner { }
public class GraveOwnerMetadata
{
    [Required(ErrorMessage = "TC Kimlik No zorunludur.")]
    [RegularExpression("^[0-9]{11}$", ErrorMessage = "TC Kimlik No 11 haneli rakam olmalıdır.")]
    public string Ssn { get; set; } = null!;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    public string? PhoneNumber { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    public string? Email { get; set; }

    [NotFutureDate(ErrorMessage = "Kayıt tarihi gelecekte olamaz.")]
    public DateOnly? RegistrationDate { get; set; }
}

[ModelMetadataType(typeof(EmployeeMetadata))]
public partial class Employee { }
public class EmployeeMetadata
{
    [Required(ErrorMessage = "Personel No zorunludur.")]
    [StringLength(20)] public string EmployeeId { get; set; } = null!;

    [Range(0, 10_000_000, ErrorMessage = "Maaş negatif olamaz.")]
    public double? Salary { get; set; }

    [NotFutureDate(ErrorMessage = "İşe giriş tarihi gelecekte olamaz.")]
    public DateOnly? HireDate { get; set; }
}

[ModelMetadataType(typeof(MonumentTypeMetadata))]
public partial class MonumentType { }
public class MonumentTypeMetadata
{
    [Required(ErrorMessage = "Anıt kodu zorunludur.")]
    [StringLength(20)] public string MonumentCode { get; set; } = null!;

    [Range(0, 50, ErrorMessage = "Maksimum yükseklik 0 ile 50 m arasında olmalıdır.")]
    public double? MaxHeight { get; set; }

    [Range(0, 50, ErrorMessage = "Taban genişliği 0 ile 50 m arasında olmalıdır.")]
    public double? BaseWidth { get; set; }
}

// ------- Capraz alan (cross-field) dogrulamalari: IValidatableObject -------

[ModelMetadataType(typeof(BurialPermitMetadata))]
public partial class BurialPermit : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (IssueDate.HasValue && ExpirationDate.HasValue && ExpirationDate < IssueDate)
            yield return new ValidationResult(
                "Geçerlilik bitiş tarihi, veriliş tarihinden önce olamaz.",
                new[] { nameof(ExpirationDate) });
    }
}
public class BurialPermitMetadata
{
    [Required(ErrorMessage = "İzin No zorunludur.")]
    [StringLength(30)] public string PermitNumber { get; set; } = null!;
}

[ModelMetadataType(typeof(ReservationMetadata))]
public partial class Reservation : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate < StartDate)
            yield return new ValidationResult(
                "Bitiş tarihi, başlangıç tarihinden önce olamaz.",
                new[] { nameof(EndDate) });
    }
}
public class ReservationMetadata
{
    [Required(ErrorMessage = "Rezervasyon No zorunludur.")]
    [StringLength(30)] public string ReservationId { get; set; } = null!;
}

[ModelMetadataType(typeof(FuneralServiceMetadata))]
public partial class FuneralService : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (StartTime.HasValue && EndTime.HasValue && EndTime < StartTime)
            yield return new ValidationResult(
                "Bitiş saati, başlangıç saatinden önce olamaz.",
                new[] { nameof(EndTime) });
    }
}
public class FuneralServiceMetadata
{
    [Required(ErrorMessage = "Servis No zorunludur.")]
    [StringLength(30)] public string ServiceId { get; set; } = null!;

    [Range(0, 100000, ErrorMessage = "Katılımcı sayısı negatif olamaz.")]
    public int? ExpectedAttendees { get; set; }
}
