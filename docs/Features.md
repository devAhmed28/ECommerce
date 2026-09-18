# Features

## Overview

E-Commerce is a modular monolith backend that provides the core functionality required for an online shopping platform.

The application is organized into independent business modules while remaining a single deployable ASP.NET Core application.

---

## 1. Identity & Authentication

The Identity module manages user accounts and authentication.

### Features

- User registration
- Email confirmation
- User login
- JWT access tokens
- Refresh tokens
- Refresh token rotation
- Refresh token revocation
- Logout
- Forgot password
- Password reset
- Change password
- Role-based authorization
- Authentication rate limiting

### Authentication Flow

````text
Register
   ↓
Email Confirmation
   ↓
Login
   ↓
Access Token + Refresh Token
   ↓
Authenticated API Requests
   ↓
Refresh Token
   ↓
New Access Token + Rotated Refresh Token

---

## 2. Catalog Management

The Catalog module manages the products and categories available in the store.

### Categories

- Create category
- Retrieve category
- List categories
- Update category
- Delete category

### Products

- Create product
- Retrieve product
- Update product
- Delete product
- Search products
- Associate products with categories

### Catalog Data Access

Catalog business data uses:

- SQL Server
- Dapper
- Explicit SQL queries

---

## 3. Shopping Cart

The Cart module manages each customer's shopping cart.

### Features

- Retrieve current cart
- Create cart when required
- Add product to cart
- Update cart item quantity
- Remove cart item
- Clear cart
- Cart ownership by authenticated user

### Cart Lifecycle

```text
Customer
   ↓
Add Product
   ↓
Cart Created
   ↓
Update / Remove Items
   ↓
Create Order
   ↓
Cart Cleared

---

## 4. Orders

The Orders module manages the customer order lifecycle.

### Features

- Create order from cart
- Retrieve an individual order
- Retrieve customer's orders
- Create order items
- Persist order status history
- Cancel pending orders
- Clear cart after successful order creation

### Order Statuses

```text
Pending
Confirmed
Processing
Shipped
Delivered
Cancelled

---

## 5. Inventory

The Inventory module manages product stock and stock reservations.

### Features

- Retrieve inventory
- Adjust stock quantity
- Reserve stock
- Release reserved stock
- Track available quantity
- Prevent reservations beyond available stock

### Inventory Quantities

```text
Available Quantity = Quantity - Reserved Quantity

Stock
  │
  ├── Available
  │
  ▼
Reserve
  │
  ▼
Reserved
  │
  ├── Order Cancelled
  │       ↓
  │    Release
  │
  └── Order continues through its lifecycle

---

## 6. Payments

The Payments module integrates with Stripe to handle payment processing and refunds.

### Features

- Create payment
- Stripe PaymentIntent integration
- Retrieve payment
- Refund payment
- Payment idempotency
- Stripe webhook processing
- Webhook signature validation
- Payment status persistence

### Payment Statuses

```text
Pending
Succeeded
Failed
Refunded

Create Payment
      ↓
Stripe PaymentIntent
      ↓
Pending
      ↓
Stripe Event
      ↓
Webhook
      ↓
Payment Status Updated
      ↓
Succeeded
      ↓
Refund
      ↓
Refunded

---

## 7. Email

Email functionality is integrated using SendGrid.

The Identity module uses email delivery for authentication-related workflows such as:

- Email confirmation
- Password reset

Sensitive SendGrid configuration is kept outside source control using application configuration and User Secrets.

---

## 8. Validation

The application uses FluentValidation for request validation.

Validation is applied at the feature level before business operations are executed.

Examples include:

- Required fields
- Product identifiers
- Category identifiers
- Positive quantities
- Valid order identifiers
- Valid payment requests
- Inventory quantities

Invalid requests are rejected before reaching the corresponding business operation.

---

## 9. Security

Security is treated as a core part of the API design.

### Authentication

- JWT bearer authentication
- Secure password hashing through ASP.NET Core Identity
- Refresh token rotation
- Refresh token revocation
- Email confirmation

### Authorization

- Role-based authorization
- Protected authenticated endpoints
- User ownership checks for user-specific resources

### Configuration Security

Sensitive configuration values are stored outside source control using User Secrets.

Examples include:

- JWT signing secret
- SendGrid API key
- Stripe secret key
- Stripe webhook secret

### Payment Security

Stripe webhook requests are validated using the configured webhook signing secret before processing events.

---

## 10. Cross-Module Integration

The application uses explicit contracts for communication between modules.

For example:

```text
Orders
   │
   │ IInventoryStockWriter
   ▼
Inventory
````
