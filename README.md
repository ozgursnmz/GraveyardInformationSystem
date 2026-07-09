# Graveyard Information System

A full‑stack **cemetery management system**: a secure **.NET 10 Web API** over a relational **Microsoft SQL Server** database, with a responsive **admin dashboard** and a **public grave‑finder** for citizens.

The system digitises the entire lifecycle of a municipal cemetery — plots, deceased records, ownership, reservations, payments, funerals, maintenance and visitors — behind a role‑based, KVKK‑aware API, and lets families locate a loved one from their phone.

---
<img width="2906" height="1646" alt="image" src="https://github.com/user-attachments/assets/c29fedfd-c712-4713-be84-48b5f9019248" />

<img width="2322" height="1532" alt="image" src="https://github.com/user-attachments/assets/3d52d761-cacc-45be-8884-53f7826cd14c" />



## Features

### Admin Dashboard (JWT‑protected)
- **Full CRUD** over 14 entities: people, deceased, grave plots, owners, employees, zones, monument types, reservations, payments, burial permits, funeral services, visitor logs, maintenance logs and app users.
- **Home dashboard** with statistic cards (total plots, occupancy rate, total deceased, income / expense / **net balance**), a **period filter** (last 1 / 3 / 6 / 12 months) and **Chart.js** charts (occupancy by zone, deaths by month, payment methods, income vs expense, maintenance cost).
- **Interactive map** (Leaflet + OpenStreetMap) of all plots, colour‑coded by status (Occupied / Available / Reserved / Maintenance), with popups and **Google Maps directions**.
- **Calendar** (FullCalendar) combining funeral services and visitor logs.
- **One‑step burial registration** — creates the person and the deceased record in a single transaction; plot & permit chosen from dropdowns.
- **Reference dropdowns** for every foreign key, so invalid IDs are impossible.
- **PDF payment receipts** (embedded Turkish‑capable font) and **Excel export** of any table.
- **Role management**, **Turkish / English** interface, **light / dark** theme, and a **responsive / mobile‑friendly** layout.

### Public Grave Finder (`find.html`, no login)
- Search by **name** and **death year**.
- Shows the **zone**, **plot number**, **map location** and a **directions** link — exactly what a visitor needs on site.

---
<img width="2366" height="1522" alt="image" src="https://github.com/user-attachments/assets/736809b5-8ffd-475d-9be8-e2320fa6f2ab" />
<img width="2354" height="1518" alt="image" src="https://github.com/user-attachments/assets/e8c8dc54-b02d-49db-8077-7bdd2843b917" />

<img width="2274" height="1446" alt="image" src="https://github.com/user-attachments/assets/da352fc1-6bc6-4049-9429-51ea7b8975af" />


## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 10 Web API, Entity Framework Core (Database‑First) |
| Auth | JWT bearer tokens, BCrypt password hashing, role‑based authorization |
| Database | Microsoft SQL Server 2022 (Docker), search‑optimised with indexes |
| Frontend | HTML + Tailwind CSS (CDN) + vanilla JavaScript |
| Libraries | Leaflet, Chart.js, FullCalendar, jsPDF, SheetJS |

The frontend is served as static files from the API's `wwwroot`, so the whole app runs from a single process.

---
<img width="2940" height="1674" alt="image" src="https://github.com/user-attachments/assets/f9ca0873-63fd-407a-a574-144885a342a4" />


## Architecture

```
Browser (Dashboard / Finder)
        │  fetch + JWT
        ▼
.NET Web API  ──►  Controllers  ──►  EF Core DbContext  ──►  SQL Server
                       │
                       └─►  DTOs (combined burial record, stats, map, calendar)
```

- **Domain hierarchy:** Cemetery Zone → Grave Plot → Deceased; Owner ↔ Reservation ↔ Payment; Deceased → Burial Permit / Funeral Service.
- **Security model:** every API read requires authentication **except** the public finder endpoint (`GET /api/GravePlots/map`), which returns only name + location — never phone numbers or national IDs. All writes require the **Admin** role.

---
<img width="2940" height="1674" alt="image" src="https://github.com/user-attachments/assets/ceee6cba-6dd5-4af5-a54c-38d2d3214e5a" />


## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for SQL Server)

### 1. Start SQL Server (Docker)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Graveyard2026" \
  -p 1433:1433 --name graveyard-sql --platform linux/amd64 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Create the database (run scripts in order)
```bash
for f in 01_schema 02_indexes 03_seed; do
  docker cp database/$f.sql graveyard-sql:/tmp/
  docker exec -it graveyard-sql /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P 'Graveyard2026' -C -i /tmp/$f.sql
done
```
- `01_schema.sql` — all tables + `APP_USER` + default admin
- `02_indexes.sql` — search / filter performance indexes
- `03_seed.sql` — ~3 years of realistic sample data

### 3. Configure secrets
Create `Graveyard.API/appsettings.Development.json` (git‑ignored):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=GraveyardBurialManagement;User Id=sa;Password=Graveyard2026;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "PUT_A_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "GraveyardAPI",
    "Audience": "GraveyardClient",
    "ExpireMinutes": "120"
  }
}
```

### 4. Run
```bash
cd Graveyard.API
dotnet run
```

| Page | URL |
|------|-----|
| Admin dashboard | http://localhost:5208/ |
| Public grave finder | http://localhost:5208/find.html |
| Swagger (API docs) | http://localhost:5208/swagger |

**Default login:** `admin` / `Admin123!`

---

## Project Structure

```
GraveyardInformationSystem/
├── database/
│   ├── 01_schema.sql        # tables + APP_USER + admin
│   ├── 02_indexes.sql       # performance indexes
│   └── 03_seed.sql          # realistic sample data (~3 years)
├── Graveyard.API/
│   ├── Controllers/         # REST endpoints (CRUD + Auth + Stats + Map…)
│   ├── Models/              # EF Core entities (Database‑First)
│   ├── Data/                # GraveyardDbContext
│   ├── Dtos/                # combined / aggregate DTOs
│   ├── wwwroot/             # frontend: index.html, find.html, login.html, app.js, i18n.js
│   ├── appsettings.json     # config (secrets kept in appsettings.Development.json)
│   └── Program.cs           # DI, JWT, static files, authorization policy
└── README.md
```

---

## Security & KVKK

- Sensitive personal data (owner phone, e‑mail, national ID, payments) is **only** reachable by authenticated staff — the API enforces a default *authenticated‑user* policy.
- The single public endpoint exposes just the deceased's name and grave location.
- Passwords are stored as **BCrypt** hashes; the JWT key and connection string are kept out of the repository.

---

## License

Educational / academic project.
