# 🎮 GameHubStore

A full-featured digital gaming platform built with **ASP.NET Core MVC 8**, **Entity Framework Core**, **ASP.NET Identity**, and **Stripe** payments.

## 🚀 Features

### Customer
- Browse, search & filter game catalog by genre
- View detailed game pages
- Add to Cart & Wishlist
- Secure checkout via Stripe
- View order history & digital invoices
- User profile dashboard (purchased games, reviews, wishlist)

### Admin
- Admin dashboard with Chart.js analytics (Monthly Revenue, Top Games)
- Full Game CRUD (Create, Edit, Delete with image uploads)
- Genre, Order, User, and Coupon management

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC 8 |
| ORM | Entity Framework Core 8 |
| Auth | ASP.NET Core Identity |
| Database | SQL Server (SQLEXPRESS) |
| Payments | Stripe (Test Mode) |
| Charts | Chart.js |
| UI | Bootstrap 5 |

## 📁 Project Structure

```
GameHubStore/
├── Areas/Admin/          # Admin panel (Controllers, Views, ViewModels)
├── Controllers/          # Public controllers (Store, Cart, Checkout, Profile)
├── Data/                 # ApplicationDbContext
├── Models/               # EF Core entities
├── ViewModels/           # View-specific models
├── Views/                # Razor views
├── Migrations/           # EF Core migrations
└── wwwroot/              # Static assets (css, js, images)
```

## ⚙️ Getting Started

1. Clone the repo
2. Update `appsettings.json` with your SQL Server connection string and Stripe test keys
3. Run migrations: `dotnet ef database update`
4. Run the app: `dotnet run`

## 💳 Stripe Setup

Replace the placeholders in `appsettings.json`:
```json
"Stripe": {
  "SecretKey": "sk_test_YOUR_KEY",
  "PublishableKey": "pk_test_YOUR_KEY"
}
```
Get your test keys from [https://dashboard.stripe.com](https://dashboard.stripe.com)
