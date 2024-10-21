# Supply Manager

## Описание проекта

Этот проект представляет собой API для управления складом, разработанное на .NET 8 с использованием ASP.NET Core Web API. API обеспечивает функциональность для управления товарами, категориями, поставщиками и заказами с использованием XML-файлов для хранения данных.

## Функциональность

API предоставляет следующие возможности:

- **CRUD операции**:
  - Создание, чтение, обновление и удаление товаров, категорий, поставщиков и заказов.

- **Фильтрация и сортировка товаров**:
  - Получение товаров по категории с возможностью сортировки по цене:
    - `GET /api/products?categoryId={categoryId}&sortBy=price&sortOrder={asc|desc}`
  
- **Фильтрация по количеству**:
  - Получение товаров с количеством меньше заданного:
    - `GET /api/products?maxQuantity={maxQuantity}`

- **Работа с заказами**:
  - Получение всех заказов для конкретного поставщика с фильтрацией по статусу:
    - `GET /api/orders?supplierId={supplierId}&status={status}`

- **Поставщики с товарами на складе**:
  - Получение списка поставщиков с минимальным количеством товаров:
    - `GET /api/suppliers?minProductQuantity={minQuantity}`

- **Заказы по датам**:
  - Получение информации о заказах по диапазону дат:
    - `GET /api/orders?startDate={startDate}&endDate={endDate}`

- **Категории с количеством товаров**:
  - Получение категории с количеством товаров в каждой категории:
    - `GET /api/categories/withProductCount`

- **Детали товара**:
  - Получение информации о товаре с его категорией и поставщиком:
    - `GET /api/products/{id}/details`

- **Пагинация для заказов**:
  - Получение всех заказов с поддержкой пагинации:
    - `GET /api/orders?pageNumber={pageNumber}&pageSize={pageSize}`

- **Пагинация для товаров**:
  - Получение всех товаров с их категориями и поставщиками с поддержкой пагинации:
    - `GET /api/products?pageNumber={pageNumber}&pageSize={pageSize}&includeDetails=true`

- **Получение популярных товаров**:
  - Получение всех товаров, которые были заказаны более 5 раз:
    - `GET /api/products/mostOrdered?minOrders=5`

- **Этот API использует асинхронное программирование, Dependency Injection для управления зависимостями и Middleware для обработки ошибок.

## Устоновка проекта
-git colone https://github.com/rustamovy9/supply-manage


