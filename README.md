# Fitness Studio Booking API

Backend engineering assessment implementation for a simplified studio booking engine.

## Tech Stack

- .NET 8 Web API
- MySQL 8
- Entity Framework Core with Pomelo MySQL provider
- Redis with StackExchange.Redis
- Swagger UI
- Clean Architecture style project split

## Projects

- `FitnessStudioBooking.Domain` - entities and enums
- `FitnessStudioBooking.Application` - DTOs, contracts, and booking business rules
- `FitnessStudioBooking.Infrastructure` - EF Core DbContext, seed data, Redis lock
- `FitnessStudioBooking.Api` - controllers and application startup
- `FitnessStudioBooking.Tests` - unit/integration test project placeholder

## Setup

Start MySQL and Redis with Docker:

```powershell
docker compose up -d
```

Or use your locally installed MySQL and Redis. Update `FitnessStudioBooking.Api/appsettings.json` if your MySQL root password is different.

Restore and build:

```powershell
dotnet restore
dotnet build
```

Create the database:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project FitnessStudioBooking.Infrastructure --startup-project FitnessStudioBooking.Api
dotnet ef database update --project FitnessStudioBooking.Infrastructure --startup-project FitnessStudioBooking.Api
```

Run the API:

```powershell
dotnet run --project FitnessStudioBooking.Api
```

Open Swagger:

```text
https://localhost:7038/swagger
```

If the port is different, check `FitnessStudioBooking.Api/Properties/launchSettings.json`.

## Required APIs

- `GET /api/packages`
- `POST /api/packages/purchase`
- `GET /api/timetable?business=1&date=2026-07-11`
- `POST /api/bookings`
- `POST /api/bookings/cancel`
- `POST /api/waitlist`

## Business Rules Covered

- Expired packages cannot be used
- Customer must have at least 1 available credit
- Package business must match schedule business
- Booking immediately deducts 1 credit
- Customer cannot book overlapping schedules
- Booking count cannot exceed available slots
- Full schedules can add customers to waitlist
- Waitlist is FIFO
- Cancellation more than 4 hours before class start refunds 1 credit
- Cancellation within 4 hours does not refund credit
- First waitlist user is promoted automatically when a booking is cancelled
- Waitlist promotion deducts 1 credit only at promotion time

## Concurrency Strategy

Booking, cancellation, and waitlist promotion use a Redis lock per timetable schedule:

```text
lock:schedule:{scheduleId}
```

The lock prevents concurrent requests from reading the same attendance count and both creating bookings. This keeps attendance from exceeding `AvailableSlots`.

Tradeoffs:

- Redis locks are simple and fast for this assessment.
- The lock has a short expiry to avoid deadlocks if the API crashes.
- In production, this should be combined with database transactions and constraints for defense in depth.
- For high scale, use a queue or command processor per schedule for even stricter ordering.

## Assumptions

- No real authentication is implemented; `CustomerId` is passed in requests.
- Mock package purchase creates credits directly without payment gateway integration.
- Waitlisted customers are not charged until promoted.
- If a waitlisted customer's package becomes expired or has no credits at promotion time, the entry is cancelled and skipped.
- Seed data uses fixed July 2026 dates so test scenarios are predictable.

## Database Schema

Main tables:

- `Businesses`
- `Customers`
- `CustomerPackages`
- `TimetableSchedules`
- `Bookings`
- `WaitlistEntries`

Relationships:

- Business has many packages and timetable schedules
- Customer has many packages, bookings, and waitlist entries
- Booking belongs to customer, package, and timetable schedule
- Waitlist entry belongs to customer, package, and timetable schedule

## Sample Test Requests

Book class:

```json
{
  "customerId": 6,
  "customerPackageId": 6,
  "timetableScheduleId": 2,
  "joinWaitlistIfFull": true
}
```

Join waitlist for seeded full class:

```json
{
  "customerId": 7,
  "customerPackageId": 7,
  "timetableScheduleId": 1
}
```

Cancel booking and promote first waitlist user:

```json
{
  "bookingId": 1
}
```
