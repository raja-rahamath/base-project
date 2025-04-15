# SaaS Application - End-to-End Guide

## Table of Contents

- [SaaS Application - End-to-End Guide](#saas-application---end-to-end-guide)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Architecture](#architecture)
  - [Backend (.NET Core API)](#backend-net-core-api)
    - [Key Features](#key-features)
    - [Project Structure](#project-structure)
    - [Database \& ORM](#database--orm)
    - [Logging](#logging)
    - [Swagger](#swagger)
    - [Installation \& First Run](#installation--first-run)
  - [Frontend (React + Vite + Material UI)](#frontend-react--vite--material-ui)
    - [Frontend Key Features](#frontend-key-features)
    - [Frontend Project Structure](#frontend-project-structure)
    - [Frontend Setup \& Run](#frontend-setup--run)
  - [End-to-End Flow](#end-to-end-flow)
  - [Testing](#testing)
  - [Deployment \& Infrastructure](#deployment--infrastructure)
  - [References](#references)

---

## Overview

This project is a multi-tenant SaaS platform with a .NET Core API backend and a React (Vite) frontend. The backend uses Entity Framework Core (code-first) with MySQL as the default database. The frontend is responsive and uses Material UI for a modern UX. The first install route initializes the database and tables, then registers the first client.

---

## Architecture

- **Backend:** .NET Core Web API, Entity Framework Core (Code-First), MySQL, Serilog logging, Swagger for API docs.
- **Frontend:** React, Vite, Material UI, communicates with API via REST.
- **Directory Structure:**
  - `api/` - Backend API
  - `frontend/` - Frontend application

---

## Backend (.NET Core API)

### Key Features

- RESTful API with controllers for install, setup, and client management.
- Entity Framework Core (code-first) for ORM.
- MySQL as the default database.
- Serilog for logging (logs stored in `api/logs/`).
- Swagger UI for API documentation and testing.
- Automatic database and table creation on first install route.

### Project Structure

- `Controllers/` - API endpoints (Install, Setup, Client)
- `Core/Entities/` - Entity models (e.g., Client, ConnectionConfig)
- `Infrastructure/Data/` - DbContext and repository pattern
- `Application/Services/` - Business logic (e.g., SaasSetupService)
- `Models/` - DTOs and response models
- `logs/` - Log files
- `appsettings.json` - Configuration (DB connection, logging, etc.)

### Database & ORM

- **Default DB:** MySQL
- **ORM:** Entity Framework Core (Code-First)
- **DbContext:** `ApplicationDbContext`
- **Migrations:** Handled automatically on first install route.

### Logging

- **Library:** Serilog
- **Location:** `api/logs/`
- **Config:** See `appsettings.json`

### Swagger

- **Enabled by default.**
- **Access:** `/swagger` endpoint when API is running.

### Installation & First Run

1. Clone the repository.
2. Configure MySQL connection in `api/appsettings.json`.
3. From the `api/` directory, run:
   ```
   dotnet run
   ```
4. Access the install route (e.g., `POST /api/install`) to:
   - Create the database and tables.
   - Register the first client.

---

## Frontend (React + Vite + Material UI)

### Frontend Key Features

- Responsive design (mobile-first).
- Material UI for consistent UX.
- Communicates with backend API via REST.
- Install, login, and registration pages.

### Frontend Project Structure

- `src/pages/` - Main pages (Install, Login, Register)
- `src/components/` - Reusable UI components (Layout, Footer)
- `src/theme.js` - Material UI theme customization
- `src/config.js` - API endpoint configuration

### Frontend Setup & Run

1. From the `frontend/` directory, install dependencies:
   ```
   npm install
   ```
2. Start the development server:
   ```
   npm run dev
   ```
3. Access the app at `http://localhost:5173` (default Vite port).

---

## End-to-End Flow

1. **First Router Install:**
   - User accesses `/install` page (frontend).
   - Frontend calls API install endpoint.
   - API creates database, tables, and registers the first client.
2. **Client Registration:**
   - After install, users can register/login.
   - All client data is stored in MySQL via EF Core.
3. **API Usage:**
   - Frontend communicates with API for all business operations.
   - Swagger UI available for API exploration.

---

## Testing

- **Backend:** Use xUnit or NUnit for API and service layer tests.
- **Frontend:** Use Vitest and React Testing Library (see `src/pages/InstallPage.test.jsx`).

---

## Deployment & Infrastructure

- **Backend:** Deploy as a container or to any .NET Core compatible host. Ensure MySQL is accessible.
- **Frontend:** Deploy static files to any web server or CDN.
- **Environment Variables:** Use `appsettings.json` and `.env` for configuration.

---

## References

- [.NET Core Docs](https://docs.microsoft.com/dotnet/core/)
- [Entity Framework Core Docs](https://docs.microsoft.com/ef/core/)
- [Serilog Docs](https://serilog.net/)
- [Swagger/OpenAPI](https://swagger.io/)
- [React](https://react.dev/)
- [Vite](https://vitejs.dev/)
- [Material UI](https://mui.com/)

---

This README provides a single source of truth for architecture, setup, and development. Update as the project evolves.
