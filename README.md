# eCommerce Products Microservice

A robust, scalable, and fully functional Products Microservice built with **ASP.NET Core (Minimal APIs)**. This service is responsible for managing eCommerce product data, including retrieving, adding, updating, and deleting products.

## 🏗️ Architecture

The project is structured using an **N-Layer Architecture** to ensure separation of concerns, maintainability, and clean code principles:

1. **Data Access Layer (DAL):** 
   - Manages database interactions using **Entity Framework Core**.
   - Implements the **Repository Pattern** (`IProductsRepository`) for abstracted data access.
   - Database Provider: **MySQL**.
2. **Business Logic Layer (BLL):** 
   - Contains the core business rules (`ProductService`).
   - Uses **DTOs** (Data Transfer Objects) for secure data shaping.
   - Integrates **AutoMapper** for seamless object mapping.
   - Implements **FluentValidation** for strong data integrity.
3. **API Layer:** 
   - Exposes RESTful endpoints using **ASP.NET Core Minimal APIs**.
   - Features a Centralized **Global Exception Handling Middleware**.

## ✨ Key Features & Bonus Implementations

- **CRUD Operations:** Complete lifecycle management for products.
- **Advanced Search:** Perform case-insensitive searches by `ProductName` or `Category` using optimized LINQ Expression Trees for server-side evaluation.
- **FluentValidation:** Strict payload validation returning HTTP `400 Bad Request` with detailed errors.
- **Global Error Handling:** Custom middleware catches unhandled exceptions, logs them securely, and returns a generic `500 Internal Server Error` to protect sensitive system details.
- **Swagger Integration (Bonus):** Auto-generated interactive API documentation.
- **CORS Configured (Bonus):** Cross-Origin Resource Sharing is fully configured to accept requests from the Angular frontend hosted on `http://localhost:4200`.

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or later)
- MySQL Server

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone <your-github-repo-url>
   cd <your-repo-folder>
