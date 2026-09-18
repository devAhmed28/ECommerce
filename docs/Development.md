# Development

## Overview

This document describes the development workflow, local setup, configuration, and conventions used when working on the E-Commerce API.

The project is developed as a modular ASP.NET Core application with SQL Server as the primary database.

---

## Prerequisites

The development environment requires:

- .NET 10 SDK
- SQL Server
- Git
- Visual Studio or another .NET-compatible IDE
- Stripe CLI for local webhook testing

Optional tooling can be used for API testing and database inspection.

---

## Repository Structure

The repository is organized into the following projects:

````text
ECommerce
│
├── src
│   ├── ECommerce.Api
│   ├── ECommerce.Modules.Catalog
│   ├── ECommerce.Modules.Identity
│   ├── ECommerce.Modules.Cart
│   ├── ECommerce.Modules.Orders
│   ├── ECommerce.Modules.Payments
│   ├── ECommerce.Modules.Inventory
│   └── ECommerce.Shared
│
└── docs

---

## Development Workflow

Development follows a feature-oriented Git workflow.

A typical workflow is:

```text
Create Feature Branch
        │
        ▼
Implement Feature
        │
        ▼
Build / Test
        │
        ▼
Commit Changes
        │
        ▼
Push Branch
        │
        ▼
Create Pull Request
        │
        ▼
Merge into develop

---

## Project References

The API project references the required business modules and shared abstractions.

The dependency structure follows the modular-monolith design:

```text
ECommerce.Api
     │
     ├── ECommerce.Modules.Catalog
     ├── ECommerce.Modules.Identity
     ├── ECommerce.Modules.Cart
     ├── ECommerce.Modules.Orders
     ├── ECommerce.Modules.Payments
     ├── ECommerce.Modules.Inventory
     └── ECommerce.Shared

     ---

## Database Development

Database changes should be treated as part of the feature that requires them.

When a feature introduces new persistence requirements, the corresponding SQL schema or database changes should be added alongside the feature implementation.

For example:

```text
Feature
   │
   ├── Application
   ├── Domain
   ├── Infrastructure
   └── Database Changes

   ---

## Running the Full Solution

The solution can be built from the repository root:

```bash
dotnet build

---

## Development Environment

The application is intended to be developed and tested locally using the .NET SDK and the configured SQL Server instance.

The main development components are:

```text
.NET 10
ASP.NET Core
SQL Server
Stripe CLI
Git
````

---

## Branch Integration

Feature development is kept isolated until the implementation and testing are complete.

The expected integration flow is:

```text
feature/<name>
      │
      ▼
Push to Remote
      │
      ▼
GitHub Pull Request
      │
      ▼
develop
```
