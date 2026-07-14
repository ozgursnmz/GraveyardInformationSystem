/* ================================================================
   SOFT DELETE / ARSIVLEME
   Ana tablolara IsArchived (bit) + ArchivedAt (tarih) kolonlari ekler.
   Calisan veritabanina guvenle uygulanabilir; veriler korunur.
   Tekrar calistirilabilir (kolon varsa atlar).
   ================================================================ */
USE GraveyardBurialManagement;
GO

IF COL_LENGTH('PERSON', 'IsArchived') IS NULL
    ALTER TABLE PERSON ADD IsArchived BIT NOT NULL DEFAULT 0, ArchivedAt DATETIME2 NULL;
GO
IF COL_LENGTH('GRAVE_PLOT', 'IsArchived') IS NULL
    ALTER TABLE GRAVE_PLOT ADD IsArchived BIT NOT NULL DEFAULT 0, ArchivedAt DATETIME2 NULL;
GO
IF COL_LENGTH('GRAVE_OWNER', 'IsArchived') IS NULL
    ALTER TABLE GRAVE_OWNER ADD IsArchived BIT NOT NULL DEFAULT 0, ArchivedAt DATETIME2 NULL;
GO
IF COL_LENGTH('DECEASED_PERSON', 'IsArchived') IS NULL
    ALTER TABLE DECEASED_PERSON ADD IsArchived BIT NOT NULL DEFAULT 0, ArchivedAt DATETIME2 NULL;
GO

PRINT 'Soft-delete kolonlari hazir.';
GO
