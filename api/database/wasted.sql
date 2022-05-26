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
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    expiration_date    TIMESTAMP WITH TIME ZONE      NOT NULL,
    code VARCHAR NOT NULL
);

CREATE TABLE reservation_items
(
    id UUID PRIMARY KEY,
    reservation_id UUID NOT NULL REFERENCES reservations (id) ON DELETE CASCADE,
    entry_id UUID NOT NULL REFERENCES offer_entries (id) ON DELETE CASCADE,
    amount INTEGER NOT NULL
);

CREATE TABLE admin_roles
(
    id UUID PRIMARY KEY,
    member_id UUID NOT NULL REFERENCES members (id) ON DELETE CASCADE
);

CREATE FUNCTION func() RETURNS TRIGGER AS $func$
    BEGIN
        IF (TG_OP = 'DELETE') THEN
            UPDATE offer_entries SET amount = amount + OLD.amount WHERE id = OLD.entry_id;
        END IF;
        RETURN NULL;
    END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER reservation_item_on_delete
AFTER DELETE ON reservation_items
    REFERENCING OLD TABLE AS reservation_items
    FOR EACH ROW EXECUTE FUNCTION func();