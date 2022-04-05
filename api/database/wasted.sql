CREATE TYPE CATEGORY_ENUM AS ENUM ('groceries', 'drinks', 'meat', 'sweets', 'other');

CREATE TABLE customers
(
    id         UUID PRIMARY KEY,
    user_name  VARCHAR NOT NULL UNIQUE,
    first_name VARCHAR NOT NULL,
    last_name  VARCHAR NOT NULL,
    email      VARCHAR NOT NULL UNIQUE,
    hash       VARCHAR NOT NULL
);

CREATE TABLE vendors
(
    id   UUID PRIMARY KEY,
    name VARCHAR NOT NULL UNIQUE
);

CREATE TABLE members
(
    id         UUID PRIMARY KEY,
    vendor_id  UUID    NOT NULL REFERENCES vendors (id) ON DELETE CASCADE,
    user_name  VARCHAR NOT NULL UNIQUE,
    first_name VARCHAR NOT NULL,
    last_name  VARCHAR NOT NULL,
    email      VARCHAR NOT NULL UNIQUE,
    hash       VARCHAR NOT NULL
);

CREATE TABLE offers
(
    id       UUID PRIMARY KEY,
    added_by UUID          NOT NULL REFERENCES vendors (id) ON DELETE CASCADE,
    name     VARCHAR       NOT NULL,
    category CATEGORY_ENUM NOT NULL,
    weight   INTEGER       NOT NULL,
    price    REAL       NOT NULL,
    UNIQUE (name, weight)
);

CREATE TABLE offer_entries
(
    id       UUID PRIMARY KEY,
    offer_id UUID      NOT NULL REFERENCES offers (id) ON DELETE CASCADE,
    expiry   DATE NOT NULL,
    added    DATE NOT NULL,
    amount   INTEGER   NOT NULL,
    UNIQUE (offer_id, expiry)
);

CREATE TABLE reservations
(
    id           UUID PRIMARY KEY,
    customer_id UUID NOT NULL REFERENCES customers (id) ON DELETE CASCADE,
    created_date DATE NOT NULL,
    expiration_date       INTEGER   NOT NULL,
    code VARCHAR NOT NULL
);

CREATE TABLE reservation_items
(
    id UUID PRIMARY KEY,
    reservation_id UUID NOT NULL REFERENCES reservations (id) ON DELETE CASCADE,
    entry_id UUID NOT NULL REFERENCES offer_entries (id) ON DELETE CASCADE,
    amount INTEGER NOT NULL
);