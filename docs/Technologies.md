# Technologies

## Overview

The E-Commerce application is built using the modern **.NET 10** ecosystem and a modular-monolith architecture.

The technology stack is selected to support:

- Modular business capabilities
- Secure authentication
- Relational data persistence
- Explicit SQL data access
- External payment processing
- Email delivery
- API documentation
- Input validation
- Production-oriented development practices

---

## Core Platform

### .NET 10

The application targets **.NET 10**.

.NET provides the runtime, SDK, dependency injection infrastructure, configuration system, and application platform used throughout the solution.

### ASP.NET Core

ASP.NET Core is used to build the HTTP API.

It provides:

- HTTP request handling
- Middleware
- Authentication
- Authorization
- Dependency injection
- Configuration
- Rate limiting
- API hosting

---

## API Documentation

### Swagger / OpenAPI

Swagger / OpenAPI is used to document and interact with the API during development.

It provides an interactive interface for:

- Viewing endpoints
- Inspecting request models
- Inspecting response models
- Supplying authentication credentials
- Executing API requests

---

## Database

### Microsoft SQL Server

SQL Server is the primary relational database used by the application.

It stores the business data for:

````text
Catalog
Cart
Orders
Inventory
Payments

---

## Email

### SendGrid

SendGrid is used as the external email provider for Identity-related email operations.

The integration supports email workflows such as:

- Email confirmation
- Password reset

The application communicates with SendGrid through the Identity module's email abstraction.

---

## Payments

### Stripe

Stripe is used for payment processing.

The Payments module integrates with Stripe PaymentIntents and refunds.

The integration supports:

- Payment creation
- PaymentIntent processing
- Payment retrieval
- Refunds
- Webhook processing
- Webhook signature validation
- Payment idempotency

Stripe CLI is used during local development to forward webhook events to the local API.

---

## API Security

### ASP.NET Core Authentication and Authorization

ASP.NET Core authentication and authorization provide the security boundary for protected API endpoints.

JWT bearer authentication identifies authenticated users, while role-based authorization restricts administrative operations.

### Rate Limiting

ASP.NET Core rate limiting is used to control excessive request traffic and protect sensitive API operations.

---

## Configuration

### ASP.NET Core Configuration

The application uses the standard ASP.NET Core configuration system for application settings.

Configuration includes areas such as:

```text
JWT
Email
Database
Stripe
SendGrid

---

## Persistence Architecture

The application uses different persistence technologies according to module responsibility.

```text
Business Modules
      │
      ▼
    Dapper
      │
      ▼
 SQL Server

Identity
      │
      ▼
Entity Framework Core
      │
      ▼
 SQL Server

 ---

## Technology Stack Summary

| Area | Technology |
|---|---|
| Runtime | .NET 10 |
| API | ASP.NET Core |
| Architecture | Modular Monolith |
| Organization | Vertical Slice |
| Database | SQL Server |
| Business Data Access | Dapper |
| Identity Persistence | Entity Framework Core |
| Authentication | ASP.NET Core Identity + JWT |
| Validation | FluentValidation |
| Payments | Stripe |
| Email | SendGrid |
| API Documentation | Swagger / OpenAPI |
| Configuration | ASP.NET Core Configuration |
| Local Secrets | User Secrets |
| Source Control | Git |
| Repository / Collaboration | GitHub |
| Payment Webhook Testing | Stripe CLI |

---

## Technology Relationships

The main technologies work together as follows:

```text
                    ASP.NET Core
                         │
              ┌──────────┴──────────┐
              │                     │
        Business Modules         Identity
              │                     │
           Dapper              EF Core / Identity
              │                     │
              └──────────┬──────────┘
                         │
                     SQL Server

Payments ───────────────► Stripe
Identity ───────────────► SendGrid

Authentication ─────────► JWT
Validation ─────────────► FluentValidation
API Documentation ──────► Swagger / OpenAPI
````
