CREATE TABLE Carts
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Carts PRIMARY KEY,

    UserId UNIQUEIDENTIFIER NOT NULL,

    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,

    CONSTRAINT UQ_Carts_UserId UNIQUE (UserId)
);

CREATE TABLE CartItems
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_CartItems PRIMARY KEY,

    CartId UNIQUEIDENTIFIER NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,

    CONSTRAINT FK_CartItems_Carts
        FOREIGN KEY (CartId)
        REFERENCES Carts(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_CartItems_Quantity
        CHECK (Quantity > 0),

    CONSTRAINT UQ_CartItems_Cart_Product
        UNIQUE (CartId, ProductId)
);

CREATE INDEX IX_CartItems_CartId
    ON CartItems(CartId);

CREATE INDEX IX_CartItems_ProductId
    ON CartItems(ProductId);