# Inventory

## Overview

The Inventory module manages product stock and stock reservations within the E-Commerce application.

Its main responsibilities are:

- Tracking product stock
- Tracking reserved stock
- Calculating available stock
- Adjusting inventory
- Reserving stock
- Releasing stock
- Coordinating inventory state with the Orders module

The module keeps inventory-related business rules inside the Inventory boundary.

---

## Inventory Model

The main inventory concepts are:

````text
InventoryItem
StockReservation

---

## Reserve Stock

Reserve Stock is used when an order requires inventory to be held for its items.

The reservation flow is:

```text
Order
  │
  ▼
Reserve Stock
  │
  ▼
Check Available Quantity
  │
  ├── Insufficient → 409 Conflict
  │
  ▼
Increase Reserved Quantity
  │
  ▼
Create Stock Reservation

---

## Orders and Inventory Integration

The Orders module integrates with Inventory through the shared `IInventoryStockWriter` abstraction.

The contract is:

```csharp
public interface IInventoryStockWriter
{
    Task ReserveAsync(
        Guid orderId,
        IReadOnlyCollection<InventoryReservationItem> items,
        CancellationToken cancellationToken);

    Task ReleaseAsync(
        Guid orderId,
        IReadOnlyCollection<InventoryReservationItem> items,
        CancellationToken cancellationToken);
}

---

## Inventory Database

The Inventory module uses SQL Server with Dapper for persistence.

The main tables are:

```text
InventoryItems
StockReservations

---

## Inventory Testing

The Inventory module was tested through the API.

The verified scenarios included:

```text
Adjust Stock
     │
     ▼
Get Inventory
     │
     ▼
Reserve Stock
     │
     ▼
Verify Reserved Quantity
     │
     ▼
Release Stock
     │
     ▼
Verify Available Quantity

````
