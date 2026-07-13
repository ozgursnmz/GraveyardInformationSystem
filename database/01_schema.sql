/* ================================================================
   Graveyard Burial Management - VERITABANI SEMASI
   Tum tablolar + APP_USER + varsayilan admin.
   Calistirma sirasi: 01_schema.sql -> 02_indexes.sql -> 03_seed.sql
   ================================================================ */
IF DB_ID('GraveyardBurialManagement') IS NULL
    CREATE DATABASE GraveyardBurialManagement;
GO
USE GraveyardBurialManagement;
GO

CREATE TABLE PERSON (
    SSN NVARCHAR(11) PRIMARY KEY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    DateOfBirth DATE,
    Gender NVARCHAR(10) DEFAULT 'Unknown',
    MotherName NVARCHAR(50) NULL,
    FatherName NVARCHAR(50) NULL
);

CREATE TABLE GRAVE_OWNER (
    SSN NVARCHAR(11) PRIMARY KEY,
    Address NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(50),
    OwnerType NVARCHAR(20) DEFAULT 'Individual',
    RegistrationDate DATE,
    Relationship NVARCHAR(50) NULL,
    FOREIGN KEY (SSN) REFERENCES PERSON(SSN)
);

CREATE TABLE EMPLOYEE (
    EmployeeID NVARCHAR(10) PRIMARY KEY,
    SSN NVARCHAR(11) UNIQUE,
    JobTitle NVARCHAR(50),
    HireDate DATE,
    Salary FLOAT CHECK (Salary >= 0),
    Shift NVARCHAR(20),
    SupervisorEmployeeID NVARCHAR(10),
    FOREIGN KEY (SSN) REFERENCES PERSON(SSN),
    FOREIGN KEY (SupervisorEmployeeID) REFERENCES EMPLOYEE(EmployeeID)
);

CREATE TABLE CEMETERY_ZONE (
    ZoneID NVARCHAR(10) PRIMARY KEY,
    Name NVARCHAR(50),
    ReligionType NVARCHAR(30),
    TotalCapacity INT,
    CurrentOccupancy INT DEFAULT 0,
    GroundType NVARCHAR(30),
    CHECK (CurrentOccupancy <= TotalCapacity)
);

CREATE TABLE MONUMENT_TYPE (
    MonumentCode NVARCHAR(10) PRIMARY KEY,
    Material NVARCHAR(30),
    Style NVARCHAR(30),
    MaxHeight FLOAT,
    BaseWidth FLOAT,
    Color NVARCHAR(20)
);

CREATE TABLE GRAVE_PLOT (
    PlotNumber NVARCHAR(15) PRIMARY KEY,
    Length FLOAT,
    Width FLOAT,
    Latitude FLOAT,
    Longitude FLOAT,
    Status NVARCHAR(20) DEFAULT 'Available',
    ZoneID NVARCHAR(10),
    MonumentCode NVARCHAR(10),
    FOREIGN KEY (ZoneID) REFERENCES CEMETERY_ZONE(ZoneID),
    FOREIGN KEY (MonumentCode) REFERENCES MONUMENT_TYPE(MonumentCode)
);

CREATE TABLE BURIAL_PERMIT (
    PermitNumber NVARCHAR(15) PRIMARY KEY,
    IssuingAuthority NVARCHAR(50),
    IssueDate DATE,
    ExpirationDate DATE,
    AuthorizedSignature NVARCHAR(50),
    CHECK (ExpirationDate > IssueDate)
);

CREATE TABLE DECEASED_PERSON (
    SSN NVARCHAR(11) PRIMARY KEY,
    DateOfDeath DATE,
    CauseOfDeath NVARCHAR(100),
    Religion NVARCHAR(30),
    VeteranStatus NVARCHAR(10) DEFAULT 'No',
    FuneralPreferences NVARCHAR(200),
    PlotNumber NVARCHAR(15) UNIQUE,
    PermitNumber NVARCHAR(15) UNIQUE,
    BurialDate DATE NULL,
    FOREIGN KEY (SSN) REFERENCES PERSON(SSN),
    FOREIGN KEY (PlotNumber) REFERENCES GRAVE_PLOT(PlotNumber),
    FOREIGN KEY (PermitNumber) REFERENCES BURIAL_PERMIT(PermitNumber)
);

CREATE TABLE FUNERAL_SERVICE (
    ServiceID NVARCHAR(15) PRIMARY KEY,
    ServiceDate DATE,
    StartTime TIME,
    EndTime TIME,
    ServiceType NVARCHAR(30),
    ExpectedAttendees INT DEFAULT 0,
    DeceasedSSN NVARCHAR(11),
    FOREIGN KEY (DeceasedSSN) REFERENCES DECEASED_PERSON(SSN),
    CHECK (ExpectedAttendees >= 0)
);

CREATE TABLE VISITOR_LOG (
    VisitID NVARCHAR(15) PRIMARY KEY,
    VisitorName NVARCHAR(100),
    VisitDate DATE,
    ArrivalTime TIME,
    DepartureTime TIME,
    Purpose NVARCHAR(100),
    PlotNumber NVARCHAR(15),
    FOREIGN KEY (PlotNumber) REFERENCES GRAVE_PLOT(PlotNumber)
);

CREATE TABLE PAYMENT (
    ReceiptNo NVARCHAR(15) PRIMARY KEY,
    Amount FLOAT CHECK (Amount > 0),
    PaymentDate DATE,
    PaymentMethod NVARCHAR(20),
    Currency NVARCHAR(10) DEFAULT 'TRY',
    BillingAddress NVARCHAR(100),
    OwnerSSN NVARCHAR(11),
    FOREIGN KEY (OwnerSSN) REFERENCES GRAVE_OWNER(SSN)
);

CREATE TABLE RESERVATION (
    ReservationID NVARCHAR(15) PRIMARY KEY,
    StartDate DATE,
    EndDate DATE,
    ReservationType NVARCHAR(20),
    Notes NVARCHAR(200),
    OwnerSSN NVARCHAR(11),
    ReceiptNo NVARCHAR(15),
    PlotNumber NVARCHAR(15) UNIQUE,
    FOREIGN KEY (OwnerSSN) REFERENCES GRAVE_OWNER(SSN),
    FOREIGN KEY (ReceiptNo) REFERENCES PAYMENT(ReceiptNo),
    FOREIGN KEY (PlotNumber) REFERENCES GRAVE_PLOT(PlotNumber),
    CHECK (EndDate IS NULL OR EndDate >= StartDate)
);

CREATE TABLE MAINTENANCE_LOG (
    PlotNumber NVARCHAR(15),
    LogNo NVARCHAR(15),
    LogDate DATE,
    TaskDescription NVARCHAR(200),
    HoursSpent FLOAT,
    Cost FLOAT,
    EmployeeID NVARCHAR(10),
    PRIMARY KEY (PlotNumber, LogNo),
    FOREIGN KEY (PlotNumber) REFERENCES GRAVE_PLOT(PlotNumber),
    FOREIGN KEY (EmployeeID) REFERENCES EMPLOYEE(EmployeeID)
);

/* Uygulama kullanicilari (JWT auth) */
CREATE TABLE APP_USER (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Admin',
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* Varsayilan admin  (kullanici: admin / sifre: Admin123!) */
IF NOT EXISTS (SELECT 1 FROM APP_USER WHERE Username='admin')
    INSERT INTO APP_USER (Username, PasswordHash, Role)
    VALUES ('admin', '$2b$11$e73qw8Ibeh/DGX37XBOQ4e1b7ZFafBMQXLU1UA2PXuiy1XudixoKm', 'Admin');
GO

/* Islem gunlugu (audit log) */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AUDIT_LOG')
    CREATE TABLE AUDIT_LOG (
        AuditID   INT IDENTITY(1,1) PRIMARY KEY,
        Username  NVARCHAR(50),
        Action    NVARCHAR(20),
        Entity    NVARCHAR(50),
        EntityKey NVARCHAR(100),
        Timestamp DATETIME NOT NULL DEFAULT GETDATE()
    );
GO
