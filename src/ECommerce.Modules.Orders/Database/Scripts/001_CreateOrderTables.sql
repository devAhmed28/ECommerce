CREATE TABLE Orders
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Orders PRIMARY KEY,

    UserId UNIQUEIDENTIFIER NOT NULL,

    TotalAmount DECIMAL(18,2) NOT NULL,

    Status INT NOT NULL
        CONSTRAINT DF_Orders_Status DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL,

    UpdatedAt DATETIME2 NOT NULL,

    CONSTRAINT CK_Orders_TotalAmount
        CHECK (TotalAmount >= 0),

    CONSTRAINT CK_Orders_Status
        CHECK (Status BETWEEN 1 AND 6)
);

CREATE INDEX IX_Orders_UserId
    ON Orders(UserId);

CREATE INDEX IX_Orders_Status
    ON Orders(Status);

CREATE INDEX IX_Orders_UserId_CreatedAt
    ON Orders(UserId, CreatedAt DESC);


CREATE TABLE OrderItems
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_OrderItems PRIMARY KEY,

    OrderId UNIQUEIDENTIFIER NOT NULL,

    ProductId INT NOT NULL,

    ProductName NVARCHAR(200) NOT NULL,

    UnitPrice DECIMAL(18,2) NOT NULL,

    Quantity INT NOT NULL,

    CONSTRAINT FK_OrderItems_Orders
        FOREIGN KEY (OrderId)
        REFERENCES Orders(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_OrderItems_UnitPrice
        CHECK (UnitPrice >= 0),

    CONSTRAINT CK_OrderItems_Quantity
        CHECK (Quantity > 0),

    CONSTRAINT UQ_OrderItems_Order_Product
        UNIQUE (OrderId, ProductId)
);

CREATE INDEX IX_OrderItems_OrderId
    ON OrderItems(OrderId);

CREATE INDEX IX_OrderItems_ProductId
    ON OrderItems(ProductId);


CREATE TABLE OrderStatusHistory
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_OrderStatusHistory PRIMARY KEY,

    OrderId UNIQUEIDENTIFIER NOT NULL,

    Status INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL,

    CONSTRAINT FK_OrderStatusHistory_Orders
        FOREIGN KEY (OrderId)
        REFERENCES Orders(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_OrderStatusHistory_Status
        CHECK (Status BETWEEN 1 AND 6)
);

CREATE INDEX IX_OrderStatusHistory_OrderId_CreatedAt
    ON OrderStatusHistory(OrderId, CreatedAt DESC);