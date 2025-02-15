﻿CREATE TABLE __EFMigrationsHistory (
    MigrationId TEXT NOT NULL PRIMARY KEY,
    ProductVersion TEXT NOT NULL
);

CREATE TABLE BankPayments (
    Id SERIAL NOT NULL PRIMARY KEY,
    Payee TEXT NOT NULL,
    Amount DECIMAL NOT NULL,
    PaymentDate TEXT NOT NULL
);

CREATE TABLE BankReceipts (
    Id SERIAL NOT NULL PRIMARY KEY,
    Payer TEXT NOT NULL,
    Amount DECIMAL NOT NULL,
    ReceiptDate TEXT NOT NULL
);

CREATE TABLE JournalEntries (
    Id SERIAL NOT NULL PRIMARY KEY,
    Description TEXT NOT NULL,
    Amount DECIMAL NOT NULL,
    EntryDate TEXT NOT NULL
);

CREATE TABLE PurchaseInvoices (
    Id SERIAL NOT NULL PRIMARY KEY,
    Supplier TEXT NOT NULL,
    Amount DECIMAL NOT NULL,
    InvoiceDate TEXT NOT NULL
);

CREATE TABLE SalesInvoices (
    Id SERIAL NOT NULL PRIMARY KEY,
    Customer TEXT NOT NULL,
    Amount DECIMAL NOT NULL,
    InvoiceDate TEXT NOT NULL
);

INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20250215123805_InitialCreate', '9.0.2');
