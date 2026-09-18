# Authentication

## Overview

Authentication is implemented using **ASP.NET Core Identity** with **JWT access tokens** and **refresh tokens**.

The Identity module is responsible for:

- User registration
- Email confirmation
- Login
- JWT access token generation
- Refresh token management
- Logout
- Password reset
- Password change
- User and role management
- Email delivery

The authentication system separates identity persistence from the business modules while exposing authentication through the main API.

---

## Authentication Architecture

The high-level authentication flow is:

````text
Client
  │
  ▼
Identity API
  │
  ▼
ASP.NET Core Identity
  │
  ├── User Store
  ├── Role Store
  └── Identity Persistence
  │
  ▼
JWT Access Token
  │
  ▼
Protected API

---

## JWT Access Tokens

The API uses JWT bearer tokens for authenticated requests.

The access token contains claims used by the application to identify and authorize the current user.

The token includes information such as:

- User identifier
- Role claims
- Issuer
- Audience
- Expiration

The configured access token lifetime is:

```text
15 minutes

---

## Password Management

The Identity module supports both password reset and authenticated password change workflows.

### Forgot Password

A user can request a password reset without providing their current password.

The flow is:

```text
Forgot Password
      │
      ▼
Generate Reset Token
      │
      ▼
Send Reset Email
      │
      ▼
User Opens Reset Link

---

## Authentication Security

The authentication system applies several security measures around credentials, tokens, and account operations.

These include:

- Short-lived JWT access tokens
- Hashed refresh-token persistence
- Refresh-token rotation
- Refresh-token revocation
- Email confirmation
- Password reset tokens
- Role-based authorization
- Rate limiting on sensitive operations
- Client IP auditing for refresh-token operations
- Secure secret storage through User Secrets

---

## Refresh Token Security

The raw refresh token is not persisted directly.

Instead, the server stores a hashed representation and validates incoming refresh tokens against the stored value.

```text
Client
  │
  │ Raw Refresh Token
  ▼
Identity API
  │
  ▼
Hash / Validate
  │
  ▼
Stored Token Hash

The complete authentication token lifecycle is:

---

## Authentication Flow Summary

The complete user authentication lifecycle can be represented as:

```text
                    Registration
                         │
                         ▼
                 Email Confirmation
                         │
                         ▼
                       Login
                         │
              ┌──────────┴──────────┐
              │                     │
              ▼                     ▼
        Access Token          Refresh Token
              │                     │
              ▼                     ▼
       Protected APIs          Token Refresh
                                    │
                                    ▼
                              Token Rotation


````
