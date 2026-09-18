# Payments

## Overview

The Payments module provides payment processing for the E-Commerce application using **Stripe**.

Its main responsibilities are:

- Creating payments
- Retrieving payment information
- Processing refunds
- Handling Stripe webhook events
- Validating webhook signatures
- Maintaining internal payment state
- Preventing duplicate payment operations through idempotency
- Integrating payment processing with Orders

The module isolates Stripe-specific implementation details inside its infrastructure layer.

---

## Payment Architecture

The high-level payment flow is:

````text
Client
  │
  ▼
Payments API
  │
  ▼
Payment Service
  │
  ├── Order Payment Contract
  │
  └── Stripe Payment Gateway
           │
           ▼
         Stripe

         ---

## Stripe PaymentIntent

Stripe PaymentIntents are used to represent payment operations with Stripe.

The application maintains the relationship between its internal payment record and the Stripe PaymentIntent.

```text
Internal Payment
      │
      ▼
Stripe PaymentIntent
      │
      ▼
Stripe Payment Processing

---

## Refunds

Payments can be refunded through:

```text
PUT /api/payments/{paymentId}/refund

---

## Orders Integration

The Payments module integrates with Orders through an application-level contract.

The payment workflow requires order information such as the amount that needs to be paid.

The integration is:

```text
Payments
    │
    ▼
Order Payment Contract
    │
    ▼
Orders
````
