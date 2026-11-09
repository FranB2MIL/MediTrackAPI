# MediTrack API

API REST de MediTrack, una plataforma para pediatras: permite cargar pacientes, registrar consultas con sus mediciones (peso, talla, IMC) y gestionar la agenda y los turnos. Trabajo final de Proyecto de Laboratorio. Integrantes: Bollea Francisco, Baccaglio Lautaro, Quiroga Tomas, Vecchia Lorenzo.

El frontend está en el repositorio [Proyecto-Pediatria-ver1](https://github.com/FranB2MIL/Proyecto-Pediatria-ver1).

## Stack

- .NET 10 (SDK 10.0.100) y ASP.NET Core Web API
- Entity Framework Core con SQLite
- Autenticación con JWT (Bearer) y contraseñas hasheadas con BCrypt
- Swagger para probar los endpoints

## Arquitectura

Clean Architecture en cuatro proyectos. Las dependencias apuntan siempre hacia adentro:

```
WebApi  →  Application  →  Domain
   └────→  Infrastructure ─→ Application
```

| Proyecto | Responsabilidad |
|---|---|
| `Domain` | Entidades (`Doctor`, `Patient`, `Admin`, `Consultation`, `Measurement`, `Availability`, `Appointment`, `DoctorPatient`) e interfaces de repositorios |
| `Application` | Servicios, DTOs, interfaces (`IPasswordHasher`, `IJwtService`) y la lógica de turnos (`SlotGenerator`) |
| `Infrastructure` | Repositorios con EF Core, `DbContext`, migraciones, implementaciones de `JwtService` y `PasswordHasher` |
| `WebApi` | Controllers, configuración de JWT y CORS, `Program.cs` |

## Roles y endpoints

Hay tres roles. El rol viaja firmado dentro del JWT y cada controller lo exige con `[Authorize(Roles = ...)]`.

| Rol | Rutas | Qué puede hacer |
|---|---|---|
| Público | `api/auth/staff/login`, `api/auth/patient/login`, `api/auth/patient/register` | Iniciar sesión (staff y pacientes tienen accesos separados) y registrarse como paciente |
| Admin | `api/admin`, `api/doctor` | Crear, editar y borrar doctores; ver pacientes; asignar o desasignar pacientes a doctores. No accede a datos clínicos |
| Doctor | `api/patient`, `api/consultation`, `api/availability`, `api/appointment` | Vincular pacientes, registrar consultas con mediciones, definir su agenda y gestionar turnos |
| Paciente | `api/me/...` | Ver su ficha e historial, editarla, ver turnos libres, reservar y cancelar turnos |

## Cómo correrlo

Requisitos: SDK de .NET 10 y la herramienta `dotnet-ef` (`dotnet tool install --global dotnet-ef`).

1. Desde la carpeta `WebApi`, cargar los secretos de desarrollo (no están en el repositorio):

```bash
dotnet user-secrets set "JwtSettings:SecretKey" "una-clave-de-al-menos-32-caracteres"
dotnet user-secrets set "SeedAdmin:Email" "admin@ejemplo.com"
dotnet user-secrets set "SeedAdmin:Password" "una-contraseña"
```

2. Crear la base de datos aplicando las migraciones (también desde `WebApi`):

```bash
dotnet ef database update --project ../Infrastructure --startup-project .
```

3. Levantar la API:

```bash
dotnet run
```

La API queda en `http://localhost:5072` y Swagger en `http://localhost:5072/swagger`. Para probar rutas protegidas, iniciá sesión, copiá el token y pegalo en el botón *Authorize* de Swagger (sin el prefijo `Bearer `).

Al arrancar, si no existe ningún admin y están cargadas las credenciales de `SeedAdmin`, se crea el administrador inicial.

El CORS solo acepta el origen `http://localhost:5173` (el servidor de desarrollo del frontend).

## Decisiones de diseño

- **Dos accesos de login**: uno para staff (admin y doctor) y otro para pacientes, así un doctor puede tener además una cuenta de paciente con el mismo mail.
- **`IAuthenticatable`**: `Admin`, `Doctor` y `Patient` la implementan, por eso la generación del JWT no depende del tipo de usuario.
- **Privacidad entre doctores**: cada consulta lleva su `DoctorId` y todas las consultas filtran por el doctor del token. Si un doctor pide un registro ajeno recibe 404, para no confirmar que existe.
- **Agenda y turnos**: el doctor define bloques semanales (`Availability`) y los turnos libres se calculan a partir de ellos; solo se guardan las reservas reales. Un índice único `(DoctorId, Date, StartTime)` sobre los turnos no cancelados evita reservas duplicadas aun con pedidos simultáneos.
- **Hora de Argentina** (UTC-3) para decidir si un turno ya pasó.

## Limitaciones conocidas

- El cálculo de **percentiles** todavía no está implementado en el backend (sí se calcula el IMC).
- No se valida el solapamiento entre bloques de disponibilidad del mismo día.
- Hay un único admin, creado por el seed, y no existe un endpoint para cambiar contraseñas.
- Borrar un doctor elimina en cascada sus consultas y turnos.