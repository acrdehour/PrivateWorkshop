# PrivateWorkshop  
A complete **Workshop Booking & Management System** built with **ASP.NET Core MVC (.NET 9)**, featuring full CRUD, role-based access control, and advanced booking management for both clients and administrators.

---

## 🧾 Project Summary  
PrivateWorkshop simulates a production-grade workshop reservation platform used by both **clients** and **administrators**. The project demonstrates key backend, frontend, and architectural skills including:

- **Authentication with ASP.NET Identity**
- **Role-based authorization**
- **Session timeout handling**
- **Repository pattern for data access**
- **Unit testing with xUnit**
- **Search, filtering, sorting, and pagination**

This application was designed as a full-stack, real-world MVC project showcasing clean architecture and .NET development best practices.

---

## ✨ Features  

### 👤 Authentication & Authorization  
- Register / Login / Logout with **ASP.NET Core Identity**  
- Role-based access (Admin / Client)  
- Auto-assign default role on registration  
- **Login session timeout** with sliding expiration  

### 🧑‍🏫 Workshop Management  
- Admin can **Create**, **Edit**, **Delete** workshops  
- **Manage booking status** directly from workshop dashboard  
- Display workshop detail and available slots  
- Validate slot capacity when booking  

### 📝 Booking System  
- Clients can **enroll** in workshops  
- Clients can **cancel** their bookings  
- Admin can **approve / reject / delete** bookings  
- View booking history with real-time status updates  

### 📄 Booking List Enhancements  
- **Sorting** (Created Date, Workshop Date)  
- **Filtering** by Booking Status  
- **Searching** by Workshop Name or Participant Email  
- **Pagination** for large datasets

---

## 🧪 Unit Testing  
The solution includes a dedicated **Unit Test Project** using **xUnit** to validate Repository functionality.

### ✔ Test Highlights  
- In-memory EF Core testing with `UseInMemoryDatabase`  
- Test coverage for:  
  - Creating bookings  
  - Fetching records  
  - Basic repository CRUD behavior  
- Fully isolated, repeatable tests
  
---

## 🧰 Technologies  
- **ASP.NET Core MVC (.NET 9)**  
- **Entity Framework Core**  
- **ASP.NET Identity (Cookie Authentication)**  
- **SQL Server**  
- **Bootstrap 5**  
- **Repository Pattern**  

---

## 📁 Project Structure
```
PrivateWorkshop/
│
├── PrivateWorkshop/ # Main ASP.NET Core MVC project
│ ├── Areas/
│ │ └── Identity/
│ │ └── Pages/ # Identity UI pages (Login, Register, etc.)
│ │
│ ├── Constants/ # Constant values (roles, configs, etc.)
│ ├── Controllers/ # MVC Controllers for Admin, Client, Guest
│ ├── Data/ # ApplicationDbContext & Seeder
│ ├── Migrations/ # EF Core migration files
│ ├── Models/ # Entity models + Enums
│ ├── Properties/ # LaunchSettings.json
│ ├── Repositories/ # Repository interfaces & implementations
│ ├── ViewModels/ # DTOs used for Views
│ ├── Views/ # Razor Views (Admin, Client, Shared)
│ ├── wwwroot/ # Static files (CSS, JS, images)
│ │
│ ├── PrivateWorkshop.csproj # Project file
│ ├── Program.cs # Main entry point
│ ├── appsettings.json # App config
│ └── appsettings.Development.json # Dev environment config
│
├── PrivateWorkshop.Tests/ # xUnit Test Project
│ ├── BookingRepositoryTests.cs
│ ├── WorkshopRepositoryTests.cs
│ └── PrivateWorkshop.Tests.csproj
│
├── assets/
│ └── screenshots/ # Screenshots used in README
│
├── .gitattributes
├── .gitignore
├── PrivateWorkshop.sln
└── README.md
```
---

## 🎯 Highlights  
- Clean, layered **MVC + Repository** architecture  
- Full CRUD and data validation workflow  
- Session expiration with sliding renewal  
- Seamless UI with search + filter + sort + pagination  
- Demonstrates real-world **role-based authorization** in .NET  
- Includes **unit-tested** repository layer for reliability
  
---

## 📸 Application Screenshots

Below is a visual overview of core system functionality.

---

### 1. Guest Experience
#### Home Page (Guest)
![Guest Home](assets/screenshots/pw-guest-home.png)

---

### 2. Client Experience
#### Home Page (Client)
![Client Home](assets/screenshots/pw-client-home.png)

#### Workshop Detail & Enrollment
![Workshop Detail](assets/screenshots/pw-client-wsdetail.png)

#### Booking Management (Client)
![Bookings](assets/screenshots/pw-client-bookings.png)

---

### 3. Admin Experience
#### Admin Dashboard
![Admin Home](assets/screenshots/pw-admin-home.png)

#### Add Workshop
![Add Workshop](assets/screenshots/pw-admin-addws.png)

---

### 4. Booking Management (Admin)
#### Sort Feature
![Sort Feature](assets/screenshots/pw-admin-bookings-sort.png)

#### Filter Feature
![Filter Feature](assets/screenshots/pw-admin-bookings-filter.png)

#### Search Feature
![Search Feature](assets/screenshots/pw-admin-bookings-search.png)

#### Pagination Feature
![Pagination Feature](assets/screenshots/pw-admin-bookings-paging.png)

---

### 5. Authentication
#### Login Page
![Login Page](assets/screenshots/pw-login.png)

#### Registration Page
![Register Page](assets/screenshots/pw-reg.png)

#### Logout Page
![Logout Page](assets/screenshots/pw-logout.png)

---

## 🚀 How to Run  
1. Update your SQL Server connection string  
2. Run database migrations  
3. Launch the application  
4. Default roles will auto-generate (Admin / Client)

---

## 🧑‍💻 Author  
**Aujchara Deeunkong**  
Open to opportunities in **.NET Backend Engineering**  
🔗 LinkedIn: www.linkedin.com/in/acrdehour-592789391
📧 Email: acrdehour@gmail.com
