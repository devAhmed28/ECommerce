# Architecture

## Overview

E-Commerce is implemented as a **modular monolith** using ASP.NET Core.

The application is deployed as a single API, while the business functionality is separated into independent modules with clear responsibilities and boundaries.

The architecture aims to keep business logic organized, reduce coupling between modules, and allow the system to evolve without immediately introducing distributed-system complexity.

---

## Architectural Style

The project combines:

- Modular Monolith
- Vertical Slice organization
- Separation of concerns
- Application-level abstractions
- Explicit module boundaries

The solution is structured around business capabilities rather than treating the entire application as one large codebase.

---

## Solution Structure

````text
ECommerce
│
├── ECommerce.Api
│
├── ECommerce.Modules.Catalog
│
├── ECommerce.Modules.Identity
│
├── ECommerce.Modules.Cart
│
├── ECommerce.Modules.Orders
│
├── ECommerce.Modules.Payments
│
├── ECommerce.Modules.Inventory
│
└── ECommerce.Shared

---

## Module Boundaries

The modules are designed around business responsibilities.

```text
Catalog
   │
   └── Products & Categories

Identity
   │
   └── Users & Authentication

Cart
   │
   └── Shopping Cart

Orders
   │
   └── Orders & Order Lifecycle

Inventory
   │
   └── Stock & Reservations

Payments
   │
   └── Payment Processing

   ---

## Module Communication

Modules communicate through explicit contracts rather than directly depending on each other's internal implementation.

For example, Orders needs to reserve and release inventory when orders are created or cancelled.

Instead of depending directly on the Inventory module's concrete implementation, Orders uses the shared abstraction:

```csharp
IInventoryStockWriter

Create Order
     │
     ▼
Validate Cart
     │
     ▼
Reserve Inventory
     │
     ├── Failure ──► Return Error
     │
     ▼
Create Order
     │
     ├── Persistence Failure ──► Release Inventory
     │
     ▼
Clear Cart
     │
     ▼
Return Order

---

## Request Flow

A typical API request passes through several layers before reaching the persistence or external-service boundary.

For example, creating an order follows this general flow:

```text
HTTP Request
     │
     ▼
Endpoint
     │
     ▼
Validation
     │
     ▼
Application Service / Use Case
     │
     ▼
Domain Logic
     │
     ▼
Infrastructure
     │
     ▼
SQL Server / External Service

---

## Transaction Boundaries

Transactions are used where multiple database operations must succeed or fail together.

Within individual modules, database operations can be executed inside a transaction to maintain consistency.

Inventory operations such as stock reservation and release use database transactions to protect inventory state.

The application also performs explicit compensation when a multi-step operation crosses module boundaries.

For example, during order creation:

```text
Reserve Inventory
       │
       ▼
Persist Order
       │
       ├── Success ──► Continue
       │
       └── Failure ──► Release Inventory

       ---

## Architectural Trade-offs

The architecture intentionally balances structure with implementation complexity.

The project does not attempt to implement every distributed-system pattern from the beginning.

For the current modular-monolith deployment model, direct in-process communication and database transactions are sufficient for the implemented business workflows.

This keeps the system easier to develop, debug, test, and deploy.

---

## Current Integration Model

The current architecture uses direct application-level contracts for cross-module operations.

```text
Orders
  │
  ├── IInventoryStockWriter
  │
  ▼
Inventory

---

## Architectural Constraints

The current architecture has several intentional constraints.

### Single Deployment

All modules run inside the same ASP.NET Core application process.

```text
                    ECommerce.Api
                         │
        ┌────────────────┼────────────────┐
        │                │                │
     Catalog          Orders          Payments
        │                │                │
        └────────────────┼────────────────┘
                         │
                    One Application

                    ---

## Architectural Goals

The architecture was designed around several practical goals.

### Separation of Responsibilities

Each module owns a specific business capability and its related implementation.

### Explicit Dependencies

Cross-module communication is performed through defined contracts instead of direct access to another module's internals.

### Maintainability

Features are organized around use cases so that related code remains easy to locate and modify.

### Simplicity

The system avoids introducing distributed infrastructure where the current modular-monolith deployment does not require it.

### Extensibility

The module boundaries provide a foundation for introducing additional infrastructure and distributed patterns as the system evolves.

---

## Final Architecture Principles

The project follows these principles:

1. **Business capabilities are separated into modules.**
2. **Features are organized around use cases.**
3. **Infrastructure concerns remain outside business logic.**
4. **Cross-module dependencies use explicit abstractions.**
5. **External services are isolated behind infrastructure implementations.**
6. **Transactions are used within appropriate consistency boundaries.**
7. **The application remains a single deployable modular monolith.**
8. **Additional distributed complexity is introduced only when required.**

These principles provide a balance between clean architecture, modularity, practical development, and production-oriented design.

````
