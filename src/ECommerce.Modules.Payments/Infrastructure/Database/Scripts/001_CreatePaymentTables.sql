USE [ECommerce];
GO

CREATE TABLE Payments
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Payments PRIMARY KEY,

    OrderId UNIQUEIDENTIFIER NOT NULL,

    UserId UNIQUEIDENTIFIER NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    Currency NVARCHAR(3) NOT NULL,

    Status INT NOT NULL,

    IdempotencyKey NVARCHAR(255) NOT NULL,

    StripePaymentIntentId NVARCHAR(100) NULL,

    StripeRefundId NVARCHAR(100) NULL,

    FailureReason NVARCHAR(500) NULL,

    CreatedAt DATETIME2 NOT NULL,

    UpdatedAt DATETIME2 NOT NULL,

    CONSTRAINT UQ_Payments_OrderId
        UNIQUE (OrderId),

    CONSTRAINT UQ_Payments_UserId_IdempotencyKey
        UNIQUE (UserId, IdempotencyKey),

    CONSTRAINT UQ_Payments_StripePaymentIntentId
        UNIQUE (StripePaymentIntentId),

    CONSTRAINT CK_Payments_Amount
        CHECK (Amount > 0),

    CONSTRAINT CK_Payments_Status
        CHECK (Status IN (1, 2, 3, 4))
);
GO

CREATE INDEX IX_Payments_UserId
    ON Payments(UserId);
GO

CREATE INDEX IX_Payments_Status
    ON Payments(Status);
GO

CREATE TABLE PaymentStatusHistory
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_PaymentStatusHistory PRIMARY KEY,

    PaymentId UNIQUEIDENTIFIER NOT NULL,

    Status INT NOT NULL,

    Reason NVARCHAR(500) NULL,

    CreatedAt DATETIME2 NOT NULL,

    CONSTRAINT FK_PaymentStatusHistory_Payments
        FOREIGN KEY (PaymentId)
        REFERENCES Payments(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_PaymentStatusHistory_Status
        CHECK (Status IN (1, 2, 3, 4))
);
GO

CREATE INDEX IX_PaymentStatusHistory_PaymentId
    ON PaymentStatusHistory(PaymentId);
GO

CREATE TABLE StripeWebhookEvents
(
    EventId NVARCHAR(100) NOT NULL
        CONSTRAINT PK_StripeWebhookEvents PRIMARY KEY,

    EventType NVARCHAR(150) NOT NULL,

    ProcessedAt DATETIME2 NOT NULL
);
GO