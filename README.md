# ChakChak Shop - Backend API

Backend проект для магазина чак-чака на .NET 8 с использованием Clean Architecture.

## Технологический стек

- **.NET 8** - основная платформа
- **PostgreSQL** - база данных
- **EF Core** - ORM для работы с БД
- **Dapper** - для сложных запросов с транзакциями
- **Redis** - кэширование
- **JWT Bearer** - аутентификация
- **API Key** - альтернативная аутентификация
- **Prometheus** - метрики
- **Grafana** - визуализация метрик
- **Serilog** - логирование
- **Swagger** - документация API
- **Liquibase** - миграции БД
- **xUnit** - unit-тестирование

## Архитектура

Проект использует Clean Architecture с разделением на слои:

- **API Layer** - контроллеры, middleware, filters
- **Application Layer** - сервисы, DTO, маппинг
- **Domain Layer** - сущности
- **Infrastructure Layer** - репозитории, DbContext, внешние сервисы
- **Tests** - unit-тесты

## Структура проекта

```
csharp-project/
├── src/
│   ├── ChakChakShop.API/          # Web API слой
│   ├── ChakChakShop.Application/    # Бизнес-логика
│   ├── ChakChakShop.Domain/        # Доменные сущности
│   └── ChakChakShop.Infrastructure/ # Инфраструктура
├── tests/
│   └── ChakChakShop.Tests/         # Unit-тесты
├── liquibase/                      # Миграции БД
├── grafana/                        # Конфигурация Grafana
└── docker-compose.yml              # Docker контейнеры
```

## Запуск проекта

### Предварительные требования

- .NET 8 SDK
- Docker и Docker Compose

### Шаги запуска

1. **Запустите все сервисы через Docker Compose:**

   ```bash
   docker compose up -d --build
   ```

   Это запустит:
   - PostgreSQL (порт 5432)
   - Redis (порт 6379)
   - Prometheus (порт 9090)
   - Grafana (порт 3000)
   - API приложение (порт 8080)

2. **Примените миграции Liquibase:**

   ```bash
   cd src/ChakChakShop.API/Data/Migrations/Liquibase
   liquibase update
   ```

   Или если используете Docker:

   ```bash
   docker run --rm -v "$(pwd)/src/ChakChakShop.API/Data/Migrations/Liquibase:/liquibase/changelog" \
     --network chakchakshop_network \
     liquibase/liquibase:latest \
     --changeLogFile=changelog/db.changelog-master.xml \
     --url=jdbc:postgresql://postgres:5432/chakchakshop \
     --username=postgres \
     --password=postgres \
     update
   ```

   После применения миграций база данных будет заполнена тестовыми данными:
   - **Роли**: Admin, Manager, Customer
   - **Категории**: Классический чак-чак, С добавками, Наборы и подарки
   - **Продукты**: 7 различных видов чак-чака с ценами и наличием на складе
   - **Тестовые пользователи** (пароль для всех: `Test123!`):
     - `admin@chakchakshop.ru` / `admin` - роль Admin
     - `manager@chakchakshop.ru` / `manager` - роль Manager
     - `customer@example.com` / `customer` - роль Customer
   - **Тестовые заказы**: 3 заказа для демонстрации

3. **Проверьте статус контейнеров:**

   ```bash
   docker compose ps
   ```

4. **Откройте Swagger UI:**
   - http://localhost:8080 (Swagger UI)
   - http://localhost:8080/swagger (альтернативный путь)

### Альтернативный запуск (без Docker для API)

Если хотите запустить API локально (без Docker):

1. **Запустите только инфраструктуру:**

   ```bash
   docker compose up -d postgres redis prometheus grafana
   ```

2. **Настройте appsettings.json:**
   - Укажите строки подключения к БД и Redis (localhost)
   - Настройте JWT ключи
   - Установите API Key

3. **Запустите приложение:**
   ```bash
   cd src/ChakChakShop.API
   dotnet run
   ```

## API Endpoints

### Аутентификация

- `POST /api/auth/register` - регистрация пользователя
- `POST /api/auth/login` - вход в систему
- `GET /api/auth/me` - информация о текущем пользователе

### Продукты

- `GET /api/products` - список продуктов (с пагинацией и фильтрацией)
- `GET /api/products/{id}` - получение продукта по ID
- `POST /api/products` - создание продукта (требует авторизации)
- `PUT /api/products/{id}` - обновление продукта (требует авторизации)
- `DELETE /api/products/{id}` - удаление продукта (требует авторизации)

### Категории

- `GET /api/categories` - список категорий
- `GET /api/categories/{id}` - получение категории по ID
- `POST /api/categories` - создание категории (требует авторизации)
- `PUT /api/categories/{id}` - обновление категории (требует авторизации)
- `DELETE /api/categories/{id}` - удаление категории (требует авторизации)

### Заказы

- `GET /api/orders` - список заказов (для админов/менеджеров)
- `GET /api/orders/my` - мои заказы
- `GET /api/orders/{id}` - получение заказа по ID
- `POST /api/orders` - создание заказа
- `PUT /api/orders/{id}/status` - обновление статуса заказа
- `DELETE /api/orders/{id}` - удаление заказа (требует авторизации)

## Роли пользователей

- **Admin** - полный доступ ко всем операциям
- **Manager** - управление заказами и продуктами
- **Customer** - просмотр и создание заказов

## Метрики и мониторинг

- **Prometheus метрики:** http://localhost:8080/metrics
- **Health checks:** http://localhost:8080/health
- **Grafana:** http://localhost:3000 (admin/admin)

## Тестирование

Запуск unit-тестов:

```bash
dotnet test
```

## Особенности реализации

### Кэширование

- GET-запросы кэшируются в Redis
- Кэш автоматически инвалидируется при обновлении/удалении данных

### Пагинация и фильтрация

- Все GET-запросы поддерживают пагинацию
- Продукты можно фильтровать по имени, категории, цене, наличию

### Обработка ошибок

- Централизованная обработка через middleware
- Единый формат ответа с кодом и описанием ошибки

### Rate Limiting

- Ограничение: 100 запросов в минуту на IP-адрес

### Idempotency

- POST-запросы поддерживают идемпотентность через заголовок `Idempotency-Key`

## Конфигурация

Основные настройки в `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=chakchakshop;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ChakChakShop",
    "Audience": "ChakChakShop",
    "ExpirationMinutes": "1440"
  },
  "ApiKey": "your-api-key-here-change-in-production"
}
```

## Лицензия

Этот проект создан в учебных целях.
