USE [ECommerce];
GO

CREATE TABLE InventoryItems
(
    ProductId INT NOT NULL PRIMARY KEY,
    Quantity INT NOT NULL,
    ReservedQuantity INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT CK_InventoryItems_Quantity
        CHECK (Quantity >= 0),

    CONSTRAINT CK_InventoryItems_ReservedQuantity
        CHECK (ReservedQuantity >= 0),

    CONSTRAINT CK_InventoryItems_ReservedNotGreaterThanQuantity
        CHECK (ReservedQuantity <= Quantity),

    CONSTRAINT FK_InventoryItems_Products
        FOREIGN KEY (ProductId)
        REFERENCES Products(Id)
);
GO

CREATE TABLE StockReservations
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ProductId INT NOT NULL,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReleasedAt DATETIME2 NULL,

    CONSTRAINT CK_StockReservations_Quantity
        CHECK (Quantity > 0),

    CONSTRAINT FK_StockReservations_InventoryItems
        FOREIGN KEY (ProductId)
        REFERENCES InventoryItems(ProductId)
);
GO

CREATE INDEX IX_StockReservations_OrderId
    ON StockReservations(OrderId);
GO

CREATE INDEX IX_StockReservations_ProductId_OrderId
    ON StockReservations(ProductId, OrderId);
GO