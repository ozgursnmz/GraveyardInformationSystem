/* ================================================================
   Arama / filtreleme performansi icin nonclustered index'ler.
   Binlerce kayitta ad-soyad, tarih, durum sorgularini hizlandirir.
   Docker'daki GraveyardBurialManagement'a calistir. Tekrar calistirilabilir.
   ================================================================ */
USE GraveyardBurialManagement;
GO

/* PERSON: ad-soyad araması (en kritik) */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Person_Name' AND object_id=OBJECT_ID('PERSON'))
    CREATE NONCLUSTERED INDEX IX_Person_Name ON PERSON (LastName, FirstName);
GO

/* DECEASED_PERSON: olum tarihi araligi + din filtresi */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Deceased_DateOfDeath' AND object_id=OBJECT_ID('DECEASED_PERSON'))
    CREATE NONCLUSTERED INDEX IX_Deceased_DateOfDeath ON DECEASED_PERSON (DateOfDeath);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Deceased_Religion' AND object_id=OBJECT_ID('DECEASED_PERSON'))
    CREATE NONCLUSTERED INDEX IX_Deceased_Religion ON DECEASED_PERSON (Religion);
GO

/* GRAVE_PLOT: durum + bolge filtresi */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_GravePlot_Status' AND object_id=OBJECT_ID('GRAVE_PLOT'))
    CREATE NONCLUSTERED INDEX IX_GravePlot_Status ON GRAVE_PLOT (Status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_GravePlot_Zone' AND object_id=OBJECT_ID('GRAVE_PLOT'))
    CREATE NONCLUSTERED INDEX IX_GravePlot_Zone ON GRAVE_PLOT (ZoneID);
GO

/* PAYMENT: tarih + sahip */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Payment_Date' AND object_id=OBJECT_ID('PAYMENT'))
    CREATE NONCLUSTERED INDEX IX_Payment_Date ON PAYMENT (PaymentDate);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Payment_Owner' AND object_id=OBJECT_ID('PAYMENT'))
    CREATE NONCLUSTERED INDEX IX_Payment_Owner ON PAYMENT (OwnerSSN);
GO

/* VISITOR_LOG: ziyaret tarihi */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Visitor_Date' AND object_id=OBJECT_ID('VISITOR_LOG'))
    CREATE NONCLUSTERED INDEX IX_Visitor_Date ON VISITOR_LOG (VisitDate);
GO

/* MAINTENANCE_LOG: log tarihi */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Maintenance_Date' AND object_id=OBJECT_ID('MAINTENANCE_LOG'))
    CREATE NONCLUSTERED INDEX IX_Maintenance_Date ON MAINTENANCE_LOG (LogDate);
GO

/* Kontrol: olusturulan index'leri listele */
SELECT t.name AS TableName, i.name AS IndexName, i.type_desc
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name LIKE 'IX_%'
ORDER BY t.name, i.name;
GO
