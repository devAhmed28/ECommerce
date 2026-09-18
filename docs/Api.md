# API

## Overview

The E-Commerce API is an ASP.NET Core Web API that exposes the application's business capabilities through HTTP endpoints.

The API is organized around business modules rather than a single global controller structure.

The main API areas are:

````text
/api
├── catalog
├── cart
├── orders
├── payments
└── identity

---

## Catalog Endpoints

### Categories

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/catalog/categories` | Create a category |
| `GET` | `/api/catalog/categories` | Get all categories |
| `GET` | `/api/catalog/categories/{id}` | Get a category |
| `PUT` | `/api/catalog/categories/{id}` | Update a category |
| `DELETE` | `/api/catalog/categories/{id}` | Delete a category |

### Products

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/catalog/products` | Create a product |
| `GET` | `/api/catalog/products/{id}` | Get a product |
| `PUT` | `/api/catalog/products/{id}` | Update a product |
| `DELETE` | `/api/catalog/products/{id}` | Delete a product |
| `GET` | `/api/catalog/products/search` | Search products |

---

## Cart Endpoints

The Cart API operates on the authenticated user's cart.

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/cart` | Get the current user's cart |
| `POST` | `/api/cart/items` | Add an item to the cart |
| `PUT` | `/api/cart/items` | Update a cart item |
| `DELETE` | `/api/cart/items` | Remove a cart item |
| `DELETE` | `/api/cart` | Clear the cart |

Cart item update and removal operations use the `cartItemId` rather than the product ID.

---

## Order Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/orders` | Create an order from the current cart |
| `GET` | `/api/orders` | Get the authenticated user's orders |
| `GET` | `/api/orders/{orderId}` | Get a specific order |
| `PUT` | `/api/orders/{orderId}/cancel` | Cancel an order |

Creating an order uses the current cart and reserves the required inventory.

Cancelling an order releases its inventory reservation.

---

## Inventory Endpoints

Inventory operations are protected and include administrative stock management.

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/inventory/{productId}` | Get inventory information |
| `PUT` | `/api/inventory/{productId}/adjust` | Adjust stock |
| `POST` | `/api/inventory/{productId}/reserve` | Reserve stock |
| `POST` | `/api/inventory/{productId}/release` | Release reserved stock |

Inventory tracks both total quantity and reserved quantity.

Available quantity is calculated as:

```text
Available Quantity = Quantity - Reserved Quantity

---

## Payment Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/payments` | Create or resume a payment |
| `GET` | `/api/payments/{paymentId}` | Get payment details |
| `PUT` | `/api/payments/{paymentId}/refund` | Refund a payment |
| `POST` | `/api/payments/webhook` | Receive Stripe webhook events |

### Payment Creation

Payment creation requires an `Idempotency-Key` header.

Example:

```http id="r6j2tu"
POST /api/payments
Authorization: Bearer <access-token>
Idempotency-Key: <unique-key>
Content-Type: application/json

---

## Request Validation

API requests are validated before the corresponding business operation is executed.

FluentValidation is used for feature-specific request validation.

Typical validation failures include:

- Required values missing
- Invalid quantities
- Invalid prices
- Invalid identifiers
- Invalid request combinations

Validation failures are returned as `400 Bad Request`.

---

## Authentication Requirements

The following operations require authentication:

```text
Cart
Orders
Payments
Account Management
Protected Inventory Operations

---

## API Security

The API applies several security measures around its HTTP endpoints.

These include:

- JWT bearer authentication
- Role-based authorization
- Request validation
- Rate limiting
- Resource ownership checks
- Stripe webhook signature validation
- Secure refresh token handling

Sensitive configuration such as JWT secrets, SendGrid credentials, and Stripe credentials is stored outside the source-controlled application configuration.

---

## Idempotency

Payment creation and refund operations support idempotency.

The client provides an `Idempotency-Key` header when creating a payment.

```http id="u4b8qf"
Idempotency-Key: <unique-request-key>

---

## API Versioning

The current API does not implement explicit URL versioning.

Endpoints therefore use the current resource paths directly, for example:

```text
/api/catalog/products
/api/cart
/api/orders
/api/payments
````
