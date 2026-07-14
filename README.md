# Graveyard Information System

[![CI](https://github.com/ozgursnmz/GraveyardInformationSystem/actions/workflows/ci.yml/badge.svg)](https://github.com/ozgursnmz/GraveyardInformationSystem/actions/workflows/ci.yml)

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
<img width="2940" height="1674" alt="image" src="https://github.com/user-attachments/assets/003ce8df-6e3b-4299-8980-fbb0901e63fd" />

- **Reference dropdowns** for every foreign key, so invalid IDs are impossible.
- **PDF payment receipts** (embedded Turkish‑capable font) and **Excel export** of any table.
- **Role management**, **Turkish / English** interface, **light / dark** theme, and a **responsive / mobile‑friendly** layout.

- <img width="2930" height="1648" alt="image" src="https://github.com/user-attachments/assets/d87889b1-8813-44c5-8bf1-90741c7006a6" />


### Public Grave Finder (`find.html`, no login)
- Search by **name** and **death year**.
- Shows the **zone**, **plot number**, **map location** and a **directions** link — exactly what a visitor needs on site.

<img width="2272" height="1404" alt="image" src="https://github.com/user-attachments/assets/393ebe92-5e01-41b5-b1ad-35a8715f4bc3" />



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

### Option A — One command (Docker Compose) ⭐ recommended

Requires only **Docker Desktop**. This builds the API, starts SQL Server, creates the schema, applies indexes and loads ~3 years of sample data automatically:

```bash
docker compose up --build
```

Then open **http://localhost:5208** (public finder) or **/index.html** (admin, `admin` / `Admin123!`).

---

### Option B — Manual (run the API locally)

#### Prerequisites
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
for f in 01_schema 02_indexes 03_seed 04_soft_delete; do
  docker cp database/$f.sql graveyard-sql:/tmp/
  docker exec -it graveyard-sql /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P 'Graveyard2026' -C -i /tmp/$f.sql
done
```
- `01_schema.sql` — all tables + `APP_USER` + default admin
- `02_indexes.sql` — search / filter performance indexes
- `03_seed.sql` — ~3 years of realistic sample data
- `04_soft_delete.sql` — archive columns for the main tables (safe on an existing DB)

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
| Public grave finder (landing) | http://localhost:5208/ |
| Admin dashboard | http://localhost:5208/index.html |
| Swagger (API docs) | http://localhost:5208/swagger |

The site opens on the **public grave finder**; use the **Login** button (top‑right) to reach the admin dashboard.

**Default admin login:** `admin` / `Admin123!`

---

## Project Structure

```
GraveyardInformationSystem/
├── database/
│   ├── 01_schema.sql        # tables + APP_USER + admin
│   ├── 02_indexes.sql       # performance indexes
│   ├── 03_seed.sql          # realistic sample data (~3 years)
│   └── 04_soft_delete.sql   # archive columns (safe ALTER for existing DBs)
├── Graveyard.API/
│   ├── Controllers/         # REST endpoints (CRUD + Auth + Stats + Map…)
│   ├── Models/              # EF Core entities (Database‑First)
│   ├── Data/                # GraveyardDbContext
│   ├── Dtos/                # combined / aggregate DTOs
│   ├── wwwroot/             # frontend: index.html, find.html, login.html, app.js, i18n.js
│   ├── appsettings.json     # config (secrets kept in appsettings.Development.json)
│   └── Program.cs           # DI, JWT, static files, authorization policy
├── Graveyard.Tests/         # xUnit unit tests (in‑memory EF Core, no SQL Server needed)
└── README.md
```

---

## Tests

Unit tests cover the dashboard statistics logic and grave‑plot CRUD, using EF Core's
in‑memory provider — so they run without a live SQL Server:

```bash
dotnet test
```

---

## Security & KVKK

- Sensitive personal data (owner phone, e‑mail, national ID, payments) is **only** reachable by authenticated staff — the API enforces a default *authenticated‑user* policy.
- The single public endpoint exposes just the deceased's name and grave location.
- Passwords are stored as **BCrypt** hashes; the JWT key and connection string are kept out of the repository.

---

## License

Educational / academic project.
