-- Создание базы данных PhotoAppDb
CREATE DATABASE PhotoAppDb;
GO

USE PhotoAppDb;
GO

-- Таблица ролей
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    role VARCHAR(50) NOT NULL UNIQUE
);
GO

-- Таблица пользователей системы
CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    role_id INT NOT NULL FOREIGN KEY REFERENCES roles(id),
    login VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);
GO

-- Таблица филиалов
CREATE TABLE branches (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    address VARCHAR(255) NOT NULL,
    phone VARCHAR(15) NOT NULL,
    workplaces INT NOT NULL DEFAULT 1
);
GO

-- Таблица киосков
CREATE TABLE kiosks (
    id INT IDENTITY(1,1) PRIMARY KEY,
    branch_id INT NOT NULL FOREIGN KEY REFERENCES branches(id),
    address VARCHAR(255) NOT NULL,
    phone VARCHAR(15) NULL
);
GO

-- Таблица клиентов
CREATE TABLE clients (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES users(id),
    phone VARCHAR(15) NOT NULL UNIQUE,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NULL,
    discount_card VARCHAR(20) NULL UNIQUE,
    is_pro BIT NOT NULL DEFAULT 0,
    personal_discount DECIMAL(5,2) DEFAULT 0.00
);
GO

-- Таблица работников
CREATE TABLE employees (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES users(id),
    full_name VARCHAR(100) NOT NULL,
    phone VARCHAR(15) NOT NULL UNIQUE,
    branch_id INT NULL FOREIGN KEY REFERENCES branches(id)
);
GO

-- Таблица услуг
CREATE TABLE services (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    base_price DECIMAL(10,2) NOT NULL,
    description TEXT NULL
);
GO

-- Таблица товаров
CREATE TABLE products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    category VARCHAR(50) NOT NULL,
    selling_price DECIMAL(10,2) NOT NULL,
    stock_quantity INT NOT NULL DEFAULT 0
);
GO

-- Таблица поставщиков
CREATE TABLE suppliers (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    contact_person VARCHAR(100) NULL,
    phone VARCHAR(15) NOT NULL,
    specialization VARCHAR(255) NULL
);
GO

-- Таблица заказов
CREATE TABLE orders (
    id INT IDENTITY(1,1) PRIMARY KEY,
    client_id INT NOT NULL FOREIGN KEY REFERENCES clients(id),
    kiosk_id INT NOT NULL FOREIGN KEY REFERENCES kiosks(id),
    branch_id INT NOT NULL FOREIGN KEY REFERENCES branches(id),
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    status VARCHAR(20) NOT NULL DEFAULT 'принят' CHECK (status IN ('принят', 'в работе', 'готов', 'выдан')),
    total_amount DECIMAL(10,2) NOT NULL,
    is_paid BIT NOT NULL DEFAULT 0
);
GO

-- Таблица услуг в заказе
CREATE TABLE order_services (
    id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL FOREIGN KEY REFERENCES orders(id),
    service_id INT NOT NULL FOREIGN KEY REFERENCES services(id),
    quantity INT NOT NULL CHECK (quantity > 0),
    unit_price DECIMAL(10,2) NOT NULL,
    is_urgent BIT DEFAULT 0,
    paper_type VARCHAR(50) NULL,
    format VARCHAR(50) NULL
);
GO

-- Таблица продаж товаров
CREATE TABLE sales (
    id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL FOREIGN KEY REFERENCES orders(id),
    product_id INT NOT NULL FOREIGN KEY REFERENCES products(id),
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    quantity INT NOT NULL CHECK (quantity > 0),
    unit_price DECIMAL(10,2) NOT NULL,
    kiosk_id INT NOT NULL FOREIGN KEY REFERENCES kiosks(id)
);
GO

-- Таблица заявок поставщикам
CREATE TABLE supply_orders (
    id INT IDENTITY(1,1) PRIMARY KEY,
    supplier_id INT NOT NULL FOREIGN KEY REFERENCES suppliers(id),
    employee_id INT NOT NULL FOREIGN KEY REFERENCES employees(id),
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    status VARCHAR(20) NOT NULL DEFAULT 'формируется' CHECK (status IN ('формируется', 'отправлена', 'получена')),
    total_amount DECIMAL(10,2) NULL
);
GO

-- Таблица позиций в заявке
CREATE TABLE supply_order_items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    supply_order_id INT NOT NULL FOREIGN KEY REFERENCES supply_orders(id),
    product_id INT NOT NULL FOREIGN KEY REFERENCES products(id),
    amount DECIMAL(10,2) NULL,
    quantity INT NOT NULL CHECK (quantity > 0)
);
GO