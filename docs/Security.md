# Security

## Overview

Security is treated as a cross-cutting concern across the E-Commerce API.

The application combines authentication, authorization, validation, secure token handling, rate limiting, secret management, resource ownership checks, and external webhook verification.

The main security boundaries are:

````text
Client
  │
  ▼
ASP.NET Core API
  │
  ├── Authentication
  ├── Authorization
  ├── Validation
  ├── Rate Limiting
  └── Resource Ownership
  │
  ▼
Business Modules
  │
  ├── SQL Server
  ├── Stripe
  └── SendGrid

  ---

## Password Security

Password management is handled by ASP.NET Core Identity.

The application supports:

- Password policy enforcement
- Current-password verification for password changes
- Password reset tokens
- Password reset through email confirmation
- Refresh-token revocation after security-sensitive account operations

The application does not implement its own password hashing mechanism.

ASP.NET Core Identity is responsible for password hashing and password verification.

---

## Email Confirmation

New accounts use email confirmation as part of the authentication lifecycle.

The flow is:

```text
Registration
     │
     ▼
Confirmation Token
     │
     ▼
Confirmation Email
     │
     ▼
Confirm Email

---

## Secret Management

Sensitive configuration is kept outside source-controlled application files.

The project uses **ASP.NET Core User Secrets** for sensitive local-development configuration.

Sensitive values include:

- JWT signing secret
- SendGrid API key
- Stripe secret key
- Stripe webhook secret

These values must never be committed to the repository.

The application accesses them through the standard ASP.NET Core configuration system.

---

## External Service Security

External integrations are isolated behind dedicated infrastructure implementations.

```text
Application
    │
    ▼
Abstraction
    │
    ▼
Infrastructure
    │
    ├── Stripe
    └── SendGrid

    ---

## Authorization Security

Authorization is enforced separately from authentication.

Authentication determines the identity associated with a request, while authorization determines whether that identity can perform the requested operation.

```text
Request
   │
   ▼
Authentication
   │
   ▼
Authenticated Identity
   │
   ▼
Authorization
   │
   ├── 403 Forbidden
   │
   ▼
Protected Operation
````
