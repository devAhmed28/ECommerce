# Database

## Overview

The E-Commerce application uses **SQL Server** as its primary relational database.

Two persistence approaches are used:

- **Dapper** for business and transactional data
- **Entity Framework Core** for ASP.NET Core Identity persistence

This allows each area of the application to use the persistence technology that fits its responsibilities while keeping the modules logically separated.

---

## Database Responsibilities

The main business modules persist their own data:

````text
SQL Server
│
├── Catalog
│   ├── Products
│   └── Categories
│
├── Cart
│   ├── Carts
│   └── CartItems
│
├── Orders
│   ├── Orders
│   └── OrderItems
│
├── Payments
│   ├── Payments
│   └── Webhook Events
│
└── Inventory
    ├── InventoryItems
    └── StockReservations

    ---

## Cart Schema

The Cart module uses two primary tables:

```text
Carts
CartItems

---

## Payments Schema

The Payments module persists payment state and webhook processing information.

The main payment data includes:

```text
Payments
Webhook Events

---

## Database Connection Management

The business modules use connection factory abstractions to create SQL Server connections.

For example:

```text
IDbConnectionFactory
        │
        ▼
SqlConnectionFactory
        │
        ▼
SqlConnection

---

## Database Configuration

Database configuration is supplied through application configuration rather than being embedded in application code.

The connection string is used by the relevant module infrastructure to establish connections to SQL Server.

Sensitive connection information should remain outside source-controlled configuration files.

For local development, sensitive configuration can be supplied through **User Secrets**.

---

## Database Initialization

Database structures are created using SQL scripts and the persistence mechanisms provided by the respective modules.

The business modules maintain their required SQL Server tables, indexes, relationships, and constraints.

Inventory, for example, requires its inventory and stock-reservation tables before inventory operations can be used.

---

## Database and Module Boundaries

Although the modules use the same SQL Server environment, logical ownership remains important.

```text
                 SQL Server
                     │
      ┌──────────────┼──────────────┐
      │              │              │
   Catalog         Orders        Inventory
      │              │              │
 Products         Orders        Stock Items
 Categories       Items         Reservations

 ---

## Data Integrity

Data integrity is protected at both the database and application levels.

The database enforces structural rules such as:

- Primary keys
- Foreign keys
- Required columns
- Unique constraints where applicable
- Indexes
- Default values

Application logic enforces business rules that cannot be represented solely through database constraints.

---

## Inventory Consistency

Inventory is one of the areas where consistency is particularly important.

The system tracks:

```text
Quantity
ReservedQuantity
AvailableQuantity

---

## Database Access Principles

The database layer follows several practical principles:

1. **Modules own their business data.**
2. **SQL access is isolated behind infrastructure code.**
3. **Dapper is used for explicit business-data queries.**
4. **Entity Framework Core is used for ASP.NET Core Identity persistence.**
5. **Transactions protect multi-step operations within a consistency boundary.**
6. **Foreign keys and database constraints protect structural integrity.**
7. **Indexes support frequently used query patterns.**
8. **Parameterized queries protect against SQL injection.**
9. **Sensitive configuration is kept outside source-controlled code.**
10. **Cross-module data should be accessed through business contracts rather than direct table manipulation.**

---

## Database Architecture Summary

The persistence architecture can be summarized as:

```text
                    Application
                        │
          ┌─────────────┴─────────────┐
          │                           │
   Business Modules                Identity
          │                           │
       Dapper                    EF Core / Identity
          │                           │
          └─────────────┬─────────────┘
                        │
                        ▼
                   SQL Server

````
