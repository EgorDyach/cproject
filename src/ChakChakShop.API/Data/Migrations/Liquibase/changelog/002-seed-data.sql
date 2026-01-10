--liquibase formatted sql

--changeset system:010-insert-default-roles
INSERT INTO roles (id, name, description) VALUES
    ('11111111-1111-1111-1111-111111111111', 'Admin', 'Administrator with full access'),
    ('22222222-2222-2222-2222-222222222222', 'Manager', 'Manager with order management access'),
    ('33333333-3333-3333-3333-333333333333', 'Customer', 'Customer with viewing and ordering access')
ON CONFLICT (name) DO NOTHING;
--rollback DELETE FROM roles WHERE name IN ('Admin', 'Manager', 'Customer');

--changeset system:011-insert-categories
INSERT INTO categories (id, name, description, created_at) VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Классический чак-чак', 'Традиционный татарский чак-чак из муки и меда', CURRENT_TIMESTAMP),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'С добавками', 'Чак-чак с орехами, сухофруктами и другими добавками', CURRENT_TIMESTAMP),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Наборы и подарки', 'Подарочные наборы и комбо-предложения', CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;
--rollback DELETE FROM categories WHERE id IN ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'cccccccc-cccc-cccc-cccc-cccccccccccc');

--changeset system:012-insert-ingredients
INSERT INTO ingredients (id, name, description) VALUES
    ('11111111-1111-1111-1111-111111111101', 'Пшеничная мука', 'Высший сорт пшеничной муки'),
    ('11111111-1111-1111-1111-111111111102', 'Мед', 'Натуральный цветочный мед'),
    ('11111111-1111-1111-1111-111111111103', 'Сливочное масло', 'Натуральное сливочное масло'),
    ('11111111-1111-1111-1111-111111111104', 'Яйца', 'Куриные яйца категории С0'),
    ('11111111-1111-1111-1111-111111111105', 'Грецкие орехи', 'Очищенные грецкие орехи'),
    ('11111111-1111-1111-1111-111111111106', 'Курага', 'Высушенные абрикосы'),
    ('11111111-1111-1111-1111-111111111107', 'Изюм', 'Коринка без косточек'),
    ('11111111-1111-1111-1111-111111111108', 'Сахар', 'Кристаллический сахар'),
    ('11111111-1111-1111-1111-111111111109', 'Ванилин', 'Натуральный ванилин')
ON CONFLICT (id) DO NOTHING;
--rollback DELETE FROM ingredients WHERE id LIKE '11111111-1111-1111-1111-1111111111%';

--changeset system:013-insert-products
INSERT INTO products (id, name, description, price, stock_quantity, category_id, created_at) VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 'Чак-чак классический 500г', 'Традиционный татарский чак-чак, приготовленный по старинному рецепту', 450.00, 25, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', CURRENT_TIMESTAMP),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 'Чак-чак классический 1кг', 'Большая порция классического чак-чака для семьи', 850.00, 15, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', CURRENT_TIMESTAMP),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', 'Чак-чак с орехами 500г', 'Чак-чак с добавлением грецких орехов', 550.00, 20, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', CURRENT_TIMESTAMP),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 'Чак-чак с сухофруктами 500г', 'Чак-чак с курагой и изюмом', 580.00, 18, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', CURRENT_TIMESTAMP),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', 'Чак-чак премиум 500г', 'Чак-чак с орехами и сухофруктами', 650.00, 12, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', CURRENT_TIMESTAMP),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc1', 'Подарочный набор "Мини"', 'Набор из классического чак-чака 500г и чак-чака с орехами 500г', 950.00, 10, 'cccccccc-cccc-cccc-cccc-cccccccccccc', CURRENT_TIMESTAMP),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc2', 'Подарочный набор "Семейный"', 'Большой набор: классический 1кг, с орехами 500г, с сухофруктами 500г', 1800.00, 8, 'cccccccc-cccc-cccc-cccc-cccccccccccc', CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;
--rollback DELETE FROM products WHERE id LIKE 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa%' OR id LIKE 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb%' OR id LIKE 'cccccccc-cccc-cccc-cccc-cccccccccccc%';

--changeset system:014-insert-product-ingredients
INSERT INTO product_ingredients (product_id, ingredient_id, quantity, unit) VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '11111111-1111-1111-1111-111111111101', 300, 'г'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '11111111-1111-1111-1111-111111111102', 150, 'г'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '11111111-1111-1111-1111-111111111103', 50, 'г'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '11111111-1111-1111-1111-111111111104', 2, 'шт'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', '11111111-1111-1111-1111-111111111101', 600, 'г'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', '11111111-1111-1111-1111-111111111102', 300, 'г'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', '11111111-1111-1111-1111-111111111103', 100, 'г'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', '11111111-1111-1111-1111-111111111104', 4, 'шт'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '11111111-1111-1111-1111-111111111101', 300, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '11111111-1111-1111-1111-111111111102', 150, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '11111111-1111-1111-1111-111111111103', 50, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '11111111-1111-1111-1111-111111111104', 2, 'шт'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '11111111-1111-1111-1111-111111111105', 100, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '11111111-1111-1111-1111-111111111101', 300, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '11111111-1111-1111-1111-111111111102', 150, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '11111111-1111-1111-1111-111111111103', 50, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '11111111-1111-1111-1111-111111111104', 2, 'шт'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '11111111-1111-1111-1111-111111111106', 80, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '11111111-1111-1111-1111-111111111107', 70, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '11111111-1111-1111-1111-111111111101', 300, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '11111111-1111-1111-1111-111111111102', 150, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '11111111-1111-1111-1111-111111111103', 50, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '11111111-1111-1111-1111-111111111104', 2, 'шт'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '11111111-1111-1111-1111-111111111105', 80, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '11111111-1111-1111-1111-111111111106', 50, 'г'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '11111111-1111-1111-1111-111111111107', 50, 'г')
ON CONFLICT (product_id, ingredient_id) DO NOTHING;
--rollback DELETE FROM product_ingredients WHERE product_id LIKE 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa%' OR product_id LIKE 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb%';

--changeset system:015-insert-test-users
-- Password for all test users: "Test123!"
-- BCrypt hash generated with cost factor 11
INSERT INTO users (id, username, email, password_hash, created_at) VALUES
    ('aaaaaaaa-0000-0000-0000-000000000001', 'admin', 'admin@chakchakshop.ru', '$2a$11$W.RDWIU9kuM1bXdUt3yY9utfpRF0OpvqDQakAhpBwS4huIsL4QRQm', CURRENT_TIMESTAMP),
    ('aaaaaaaa-0000-0000-0000-000000000002', 'manager', 'manager@chakchakshop.ru', '$2a$11$W.RDWIU9kuM1bXdUt3yY9utfpRF0OpvqDQakAhpBwS4huIsL4QRQm', CURRENT_TIMESTAMP),
    ('aaaaaaaa-0000-0000-0000-000000000003', 'customer', 'customer@example.com', '$2a$11$W.RDWIU9kuM1bXdUt3yY9utfpRF0OpvqDQakAhpBwS4huIsL4QRQm', CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;
--rollback DELETE FROM users WHERE id LIKE 'aaaaaaaa-0000-0000-0000-00000000000%';

--changeset system:016-insert-user-roles
INSERT INTO user_roles (user_id, role_id) VALUES
    ('aaaaaaaa-0000-0000-0000-000000000001', '11111111-1111-1111-1111-111111111111'),
    ('aaaaaaaa-0000-0000-0000-000000000002', '22222222-2222-2222-2222-222222222222'),
    ('aaaaaaaa-0000-0000-0000-000000000003', '33333333-3333-3333-3333-333333333333')
ON CONFLICT (user_id, role_id) DO NOTHING;
--rollback DELETE FROM user_roles WHERE user_id LIKE 'aaaaaaaa-0000-0000-0000-00000000000%';

--changeset system:017-insert-test-orders
INSERT INTO orders (id, user_id, total_amount, status, created_at) VALUES
    ('bbbbbbbb-0000-0000-0000-000000000001', 'aaaaaaaa-0000-0000-0000-000000000003', 1030.00, 'Completed', CURRENT_TIMESTAMP - INTERVAL '2 days'),
    ('bbbbbbbb-0000-0000-0000-000000000002', 'aaaaaaaa-0000-0000-0000-000000000003', 1230.00, 'Processing', CURRENT_TIMESTAMP - INTERVAL '1 day'),
    ('bbbbbbbb-0000-0000-0000-000000000003', 'aaaaaaaa-0000-0000-0000-000000000003', 450.00, 'Pending', CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;
--rollback DELETE FROM orders WHERE id LIKE 'bbbbbbbb-0000-0000-0000-00000000000%';

--changeset system:018-insert-order-items
INSERT INTO order_items (id, order_id, product_id, quantity, unit_price, total_price) VALUES
    ('cccccccc-0000-0000-0000-000000000001', 'bbbbbbbb-0000-0000-0000-000000000001', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 1, 450.00, 450.00),
    ('cccccccc-0000-0000-0000-000000000002', 'bbbbbbbb-0000-0000-0000-000000000001', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', 1, 550.00, 550.00),
    ('cccccccc-0000-0000-0000-000000000003', 'bbbbbbbb-0000-0000-0000-000000000002', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 1, 580.00, 580.00),
    ('cccccccc-0000-0000-0000-000000000004', 'bbbbbbbb-0000-0000-0000-000000000002', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', 1, 650.00, 650.00),
    ('cccccccc-0000-0000-0000-000000000005', 'bbbbbbbb-0000-0000-0000-000000000003', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 1, 450.00, 450.00)
ON CONFLICT (id) DO NOTHING;
--rollback DELETE FROM order_items WHERE id LIKE 'cccccccc-0000-0000-0000-00000000000%';
