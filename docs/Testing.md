# Testing

## Overview

The E-Commerce application uses a workflow-oriented testing approach focused on verifying complete business scenarios through the API.

Testing covers:

- Authentication
- Catalog
- Cart
- Orders
- Inventory
- Payments
- Cross-module integration
- Validation
- Authorization
- External payment webhooks

The project emphasizes end-to-end verification of important business workflows in addition to build verification.

---

## Testing Strategy

The overall testing approach can be represented as:

````text
Code
 │
 ▼
Build Verification
 │
 ▼
Feature Testing
 │
 ▼
Integration Testing
 │
 ▼
End-to-End Workflow Testing

---

---

## Authentication Testing

The authentication workflow was verified through the API.

The main scenarios include:

```text
Register
   │
   ▼
Confirm Email
   │
   ▼
Login
   │
   ▼
Access Token
   │
   ▼
Refresh Token
   │
   ▼
Logout

---

## Orders Testing

Orders were tested through a complete cart-to-order workflow.

The verified scenario was:

```text
Cart with Items
      │
      ▼
Create Order
      │
      ▼
Order Created
      │
      ▼
Get Order
      │
      ▼
Get Customer Orders
      │
      ▼
Cancel Order
      │
      ▼
Get Order
      │
      ▼
Verify Cancelled Status

---

## Payments Testing

The Payments module was tested through a complete Stripe payment lifecycle.

The verified workflow was:

```text
Create Payment
      │
      ▼
Stripe PaymentIntent
      │
      ▼
Confirm Test Payment
      │
      ▼
Stripe Webhook
      │
      ▼
Payment → Succeeded
      │
      ▼
Refund Payment
      │
      ▼
Stripe Refund Webhooks
      │
      ▼
Payment → Refunded

---

## Authorization Testing

Authorization was tested for protected operations.

The tests verify that:

- Unauthenticated requests cannot access protected endpoints.
- Authenticated users can access operations permitted to them.
- Administrative operations require the appropriate role.
- Users cannot access resources belonging to another user.

The authorization flow is:

```text
Request
   │
   ▼
Authentication
   │
   ▼
Authorization
   │
   ├── Unauthorized → 401
   │
   ├── Forbidden → 403
   │
   ▼
Endpoint
````
