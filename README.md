# 🛒 Ecommerce.API

A modern RESTful E-Commerce API built with **ASP.NET Core 10**, following clean backend practices such as Repository Pattern, JWT Authentication, Refresh Tokens, Role-Based Authorization, and Stripe Payment Integration.

This project provides a complete backend solution for an online shopping platform, including product management, shopping cart, orders, reviews, promotions, favorites, authentication, and payment processing.

---

# 🚀 Features

## Authentication & Authorization
- User Registration
- User Login
- JWT Authentication
- Refresh Token Authentication
- Logout (Refresh Token Revocation)
- Forgot Password
- Reset Password
- Role-Based Authorization
- Identity Framework

---

## Products

- Create Product
- Update Product
- Delete Product
- Get Product By Id
- Get All Products
- Product Filtering
- Product Pagination
- Product Images

---

## Categories

- CRUD Operations

---

## Brands

- CRUD Operations

---

## Shopping Cart

- Add Product to Cart
- Update Quantity
- Remove Product
- Get Cart
- Apply Promotion Codes
- Stripe Checkout Integration

---

## Promotions

- Percentage Discount
- Product-Specific Promotions
- Cart-Wide Promotions

---

## Orders

- Create Orders
- Order History
- Order Filtering
- Payment Status
- Order Status Tracking
- Stripe Payment
- Checkout Success Handling

---

## Favorites

- Add to Favorites
- Remove from Favorites
- Get Favorite Products

---

## Reviews

- Add Review
- Update Review
- Delete Review
- Get Product Reviews
- Only Verified Buyers Can Review
- One Review Per User Per Product

---

## User Profile

- Update Profile
- Change Password
- Get Current User

---

# 🔐 Security

- ASP.NET Identity
- JWT Access Tokens
- Refresh Tokens
- Refresh Token Rotation
- Refresh Token Hashing
- Role-Based Authorization
- Password Validation

---

# 💳 Payment

Integrated with **Stripe Checkout**

Features:

- Secure Checkout Session
- Payment Confirmation
- Order Creation
- Transaction Id Storage

---

# 🛠️ Technologies

- ASP.NET Core 10
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication
- Stripe.NET
- Repository Pattern
- LINQ
- REST API
- Swagger / OpenAPI
- Scalar API

---

# 📂 Project Structure

```
Controllers/
Services/
Repositories/
Models/
DTOs/
Utility/
Data/
Images/
```

---

# API Features

- RESTful Design
- Pagination
- Filtering
- Image Upload
- Model Validation
- Logging
- Repository Pattern
- Async/Await

---

# Getting Started

## Clone

```bash
git clone https://github.com/YourUserName/Ecommerce.API.git
```

## Install Packages

```bash
dotnet restore
```

## Update Database

```bash
dotnet ef database update
```

## Run

```bash
dotnet run
```

---

# Configuration

Configure the following in **appsettings.json** or **User Secrets**

```json
ConnectionStrings

JWT:
    Secret
    Issuer
    Audience

Stripe:
    SecretKey
```

---

# Future Improvements

- Clean Architecture
- AutoMapper
- Redis Caching
- Email Notifications
- Product Search
- Wishlist Enhancements
- Soft Delete
- Audit Logs
- Unit Testing
- Docker Support
- Background Jobs (Hangfire)
- Rate Limiting

---

# Author

**Mohamed Ahmed Hussein**

Backend Developer (.NET)

GitHub:
[https://github.com/YourGitHub](https://github.com/mohamed-A-huss)

LinkedIn:
[https://linkedin.com/in/YourLinkedIn](https://www.linkedin.com/in/mohamedahmed1080/)

---
