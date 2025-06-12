
# Store – ASP.NET Core Online Store

**Store** is a modern e-commerce web application built with **ASP.NET Core**, powered by **Tailwind CSS** for a responsive UI, and using **MySQL Server** for database storage. The project architecture supports clean separation via Controllers, Razor pages, and REST APIs.

---

## 🌟 Features

- **Product Listing**: Browse items with images, prices, and descriptions  
- **Product Details**: View detailed product info on single-product pages  
- **Shopping Cart**: Add/remove items and view cart contents  
- **Secure Authentication**: ASP.NET Identity-based login and registration  
- **Admin Panel**: Manage products, categories, and orders  
- **Responsive Design**: Mobile-first interface with Tailwind CSS

---

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core (.NET 7+)  
- **Frontend**: Razor Views + Tailwind CSS  
- **Database**: MySQL Server  
- **ORM**: Entity Framework Core  
- **Build Tools**: NPM, Tailwind CLI  

---

## ⚙️ Installation & Setup

### 1. Prerequisites

- .NET SDK 7.0 or higher  
- MySQL Server (configured database + user)  
- Node.js (with NPM)

---

### 2. Clone the Repository

```bash
git clone https://github.com/wijdane8/Store.git
cd Store
```

---

### 3. Backend Setup

```bash
dotnet restore
```

- Configure your `appsettings.json` with your MySQL connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=StoreDb;User=root;Password=YOUR_PASS;"
}
```

- Apply migrations and run the application:

```bash
dotnet ef database update
dotnet run
```

The backend server will run at https://localhost:5001 (or a similar URL).

---

### 4. Frontend (Tailwind CSS) Setup

Open a new terminal window inside the same project directory:

```bash
npm install
```

- Add the following script to your `package.json` inside the `"scripts"` section:

```json
"scripts": {
  "build:css": "npx tailwindcss -o wwwroot/css/site.css --minify"
}
```

- Build the Tailwind CSS file:

```bash
npm run build:css
```

- For development with automatic rebuild on changes, use watch mode:

```bash
npx tailwindcss -o wwwroot/css/site.css --watch
```

- Restart the ASP.NET server after CSS changes to see updates.

---

## 🧩 Tailwind Integration Highlights

Tailwind scans `.cshtml` Razor views for classes using this config:

```js
content: ["Views/**/*.cshtml", "Pages/**/*.cshtml", "wwwroot/js/**/*.js"]
```

CSS is built via Tailwind CLI or PostCSS, following best practices for ASP.NET Core + Tailwind integration.

---

## 📄 License

This project is licensed under the MIT License. See the LICENSE file for details.

