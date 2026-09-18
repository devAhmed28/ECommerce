# ECommerce - E-Commerce Platform API

ECommerce is a production-minded **E-Commerce backend** built with **C# and ASP.NET Core Web API** as a portfolio project.

The project is designed as a **modular monolith**, connecting core business capabilities such as **Identity, Catalog, Cart, Orders, Inventory, and Payments** while keeping each module isolated behind clear boundaries.

The application covers the main E-Commerce workflow from **authentication and product management to cart, order, inventory, payment, refund, and email operations**.

---

## Features

- User Registration & Authentication
- Email Confirmation
- Password Reset & Change
- JWT Access Tokens
- Refresh Token Rotation & Revocation
- Role-Based Authorization
- Product & Category Management
- Product Search
- Shopping Cart Management
- Order Creation & Cancellation
- Inventory Management
- Stock Reservations
- Stripe Payment Processing
- Payment Refunds
- Stripe Webhook Processing
- Transactional Email Delivery with SendGrid
- FluentValidation
- API Rate Limiting
- Swagger / OpenAPI Documentation

📄 **[Full Features Documentation](docs/Features.md)**

---

## Architecture

- Modular Monolith Architecture
- Vertical Slice Organization
- Clear Module Boundaries
- Separation of Responsibilities
- Explicit Cross-Module Contracts
- Dependency Injection
- Dapper for Business Data Access
- Entity Framework Core for Identity Persistence
- Transactional Database Operations
- Compensation for Cross-Module Inventory Operations
- Global Exception Handling
- FluentValidation
- JWT Authentication
- Role-Based Authorization
- Resource Ownership Checks
- Payment Idempotency
- Stripe Webhook Signature Validation
- Secure Secret Management

📄 **[Architecture Documentation](docs/Architecture.md)**

---

## Technologies

| Category              | Technologies                                        |
| --------------------- | --------------------------------------------------- |
| **Language**          | C#                                                  |
| **Framework**         | ASP.NET Core Web API, .NET 10                       |
| **Architecture**      | Modular Monolith, Vertical Slices                   |
| **Database**          | SQL Server                                          |
| **Data Access**       | Dapper, Entity Framework Core                       |
| **Authentication**    | ASP.NET Core Identity, JWT, Refresh Tokens          |
| **Authorization**     | Role-Based Authorization, Resource Ownership Checks |
| **Validation**        | FluentValidation                                    |
| **Payments**          | Stripe PaymentIntents, Stripe Webhooks              |
| **Email**             | SendGrid                                            |
| **API Documentation** | Swagger / OpenAPI                                   |
| **Development Tools** | Git, Stripe CLI, User Secrets                       |

📄 **[Full Tech Stack Details](docs/Technologies.md)**

---

## API Endpoints

| Module        | Available Operations                                                                    |
| ------------- | --------------------------------------------------------------------------------------- |
| **Identity**  | Registration, Email Confirmation, Login, Token Refresh, Logout, Password Reset & Change |
| **Catalog**   | Category Management, Product Management, Product Search                                 |
| **Cart**      | Cart Retrieval, Add Items, Update Items, Remove Items, Clear Cart                       |
| **Orders**    | Create Order, Get Order, Get Customer Orders, Cancel Order                              |
| **Inventory** | Stock Management, Stock Reservations, Reservation Release                               |
| **Payments**  | Payment Creation, Payment Retrieval, Refunds, Stripe Webhooks                           |

📄 **[Complete API Documentation](docs/Api.md)**

---

## Database

- SQL Server
- Dapper for Catalog and business-oriented data access
- Entity Framework Core for ASP.NET Core Identity persistence
- Transactional database operations
- Inventory reservations
- Payment state persistence
- Payment webhook event persistence
- Refresh token persistence

The database contains tables for the major business areas, including:

- Categories
- Products
- Carts
- CartItems
- Orders
- OrderItems
- InventoryItems
- StockReservations
- Payments
- PaymentWebhookEvents
- Identity tables
- Refresh Tokens

📄 **[Database Schema Documentation](docs/Database.md)**

---

## Authentication & Security

### Authentication

- User Registration
- Email Confirmation
- JWT Access Tokens
- Refresh Tokens
- Refresh Token Rotation
- Refresh Token Revocation
- Password Reset
- Password Change

The authentication lifecycle follows:

````text
Register
   │
   ▼
Confirm Email
   │
   ▼
Login
   │
   ├── Access Token
   │
   └── Refresh Token
           │
           ▼
      Refresh / Rotate
           │
           ▼
      New Token Pair

      ## Business Flow

### Order & Inventory

The order workflow integrates the Cart, Orders, and Inventory modules:

```text
Cart
  │
  ▼
Create Order
  │
  ▼
Reserve Inventory
  │
  ▼
Persist Order

## External Integrations

The application integrates with external services for payment processing and transactional email delivery.

### Stripe

Used for:

- PaymentIntents
- Payment Confirmation
- Refunds
- Payment Webhooks
- Webhook Signature Validation

### SendGrid

Used for:

- Transactional Email Delivery
- Authentication-related Email Workflows

Sensitive integration credentials are stored through application configuration and **User Secrets** during development.

📄 **[Payments Documentation](docs/Payments.md)**
📄 **[Security Documentation](docs/Security.md)**

---

## Testing

The application was verified through **end-to-end API scenarios** covering the main business workflows.

### Verified Areas

- Identity registration and authentication
- Token refresh and logout
- Password workflows
- Catalog operations
- Cart operations
- Order creation
- Order cancellation
- Inventory reservation
- Inventory release
- Insufficient-stock handling
- Payment creation
- Stripe payment confirmation
- Payment retrieval
- Payment refunds
- Stripe webhook processing

📄 **[Testing Documentation](docs/Testing.md)**

---

## Getting Started

### Prerequisites

Make sure the following are installed:

- .NET 10 SDK
- SQL Server
- Git
- Stripe CLI for local webhook testing

A development environment capable of running HTTPS locally is also required.

### Clone the Repository

```bash
git clone https://github.com/devAhmed28/ECommerce.git
cd ECommerce

## Development Workflow

The project follows a feature-focused development workflow:

```text
Create Feature
      │
      ▼
Implement Vertical Slice
      │
      ▼
Add Validation
      │
      ▼
Build
      │
      ▼
Run API
      │
      ▼
Test API
      │
      ▼
Review Changes
      │
      ▼
Commit

## Project Status

The core E-Commerce backend has been implemented with the major business modules and integrations described above.

### ✅ Implemented

- Modular monolith architecture
- Core E-Commerce modules
- Authentication and authorization
- Catalog and product management
- Shopping cart
- Orders
- Inventory and stock reservations
- Stripe payments and refunds
- Stripe webhook processing
- SendGrid email integration
- Request validation
- API rate limiting
- Secure secret management
- End-to-end API verification
- Swagger / OpenAPI documentation

## Documentation

More detailed information about the project is available in the `docs/` directory.

| Document | Description |
| -------- | ----------- |
| [Features](docs/Features.md) | Complete feature list |
| [Architecture](docs/Architecture.md) | Architecture and design decisions |
| [Technologies](docs/Technologies.md) | Technology stack and tools |
| [API](docs/Api.md) | Complete API documentation |
| [Database](docs/Database.md) | Database structure and persistence strategy |
| [Authentication](docs/Authentication.md) | Authentication and authorization |
| [Testing](docs/Testing.md) | Testing approach and scenarios |
| [Security](docs/Security.md) | Security practices and secret management |
| [Inventory](docs/Inventory.md) | Inventory and stock reservation |
| [Payments](docs/Payments.md) | Stripe payment integration |
| [Development](docs/Development.md) | Development workflow |

---

## Author

**Ahmed Rabie**

- 📧 Email: `devea7med@gmail.com`
- 🐙 GitHub: [@devAhmed28](https://github.com/devAhmed28)

---

## License

This project is licensed under the MIT License.

---

**Made with ❤️ by Ahmed Rabie**
````
