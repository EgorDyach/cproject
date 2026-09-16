--liquibase formatted sql

--changeset system:001-create-users-table
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_username ON users(username);
--rollback DROP INDEX IF EXISTS idx_users_username; DROP INDEX IF EXISTS idx_users_email; DROP TABLE IF EXISTS users CASCADE;

--changeset system:002-create-roles-table
CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(500)
);
--rollback DROP TABLE IF EXISTS roles CASCADE;

--changeset system:003-create-user-roles-table
CREATE TABLE user_roles (
    user_id UUID NOT NULL,
    role_id UUID NOT NULL,
    PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE
);
--rollback DROP TABLE IF EXISTS user_roles CASCADE;

--changeset system:004-create-categories-table
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);
--rollback DROP TABLE IF EXISTS categories CASCADE;

--changeset system:005-create-products-table
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    description TEXT,
    price DECIMAL(18,2) NOT NULL,
    stock_quantity INTEGER NOT NULL,
    category_id UUID NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    CONSTRAINT fk_products_category FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE RESTRICT
);

CREATE INDEX idx_products_category ON products(category_id);
--rollback DROP INDEX IF EXISTS idx_products_category; DROP TABLE IF EXISTS products CASCADE;

--changeset system:006-create-ingredients-table
CREATE TABLE ingredients (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    description TEXT
);
--rollback DROP TABLE IF EXISTS ingredients CASCADE;

--changeset system:007-create-product-ingredients-table
CREATE TABLE product_ingredients (
    product_id UUID NOT NULL,
    ingredient_id UUID NOT NULL,
    quantity DECIMAL(18,2) NOT NULL,
    unit VARCHAR(50) NOT NULL,
    PRIMARY KEY (product_id, ingredient_id),
    CONSTRAINT fk_product_ingredients_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
    CONSTRAINT fk_product_ingredients_ingredient FOREIGN KEY (ingredient_id) REFERENCES ingredients(id) ON DELETE CASCADE
);
--rollback DROP TABLE IF EXISTS product_ingredients CASCADE;

--changeset system:008-create-orders-table
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    total_amount DECIMAL(18,2) NOT NULL,
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    CONSTRAINT fk_orders_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT
);

CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_orders_created_at ON orders(created_at DESC);
--rollback DROP INDEX IF EXISTS idx_orders_created_at; DROP INDEX IF EXISTS idx_orders_user; DROP TABLE IF EXISTS orders CASCADE;

--changeset system:009-create-order-items-table
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL,
    product_id UUID NOT NULL,
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(18,2) NOT NULL,
    total_price DECIMAL(18,2) NOT NULL,
    CONSTRAINT fk_order_items_order FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    CONSTRAINT fk_order_items_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT
);

CREATE INDEX idx_order_items_order ON order_items(order_id);
CREATE INDEX idx_order_items_product ON order_items(product_id);
--rollback DROP INDEX IF EXISTS idx_order_items_product; DROP INDEX IF EXISTS idx_order_items_order; DROP TABLE IF EXISTS order_items CASCADE;

