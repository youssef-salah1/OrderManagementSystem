# Quick Start Guide - Testing the Order Management System

## 🚀 Getting Started in 5 Minutes

### Step 1: Run the Application
```bash
cd OrderManagementSystem
dotnet run
```

### Step 2: Open Swagger UI
Open your browser and navigate to:
```
https://localhost:5001/swagger
```

---

## 📝 Sample Test Scenario

Follow these steps in Swagger UI to test the complete workflow:

### 1️⃣ Register an Admin User

**Endpoint:** `POST /api/users/register`

**Request Body:**
```json
{
  "username": "admin@test.com",
  "password": "Admin123!",
  "role": "Admin"
}
```

**Expected Response:** `200 OK`
```json
{
  "userId": 1,
  "username": "admin@test.com"
}
```

---

### 2️⃣ Login as Admin

**Endpoint:** `POST /api/users/login`

**Request Body:**
```json
{
  "username": "admin@test.com",
  "password": "Admin123!"
}
```

**Expected Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "Admin",
  "userId": 1
}
```

**⚠️ IMPORTANT:** Copy the token value!

---

### 3️⃣ Authorize in Swagger

1. Click the **"Authorize"** button (🔒) at the top of Swagger UI
2. Enter: `Bearer {paste-your-token-here}`
3. Click **"Authorize"**
4. Click **"Close"**

Now you're authenticated! 🎉

---

### 4️⃣ Create Products

**Endpoint:** `POST /api/products`

**Product 1 - Laptop:**
```json
{
  "name": "Dell XPS 15 Laptop",
  "price": 1500.00,
  "stock": 20
}
```

**Product 2 - Mouse:**
```json
{
  "name": "Logitech MX Master Mouse",
  "price": 99.99,
  "stock": 100
}
```

**Product 3 - Keyboard:**
```json
{
  "name": "Mechanical Keyboard",
  "price": 150.00,
  "stock": 50
}
```

**Expected Response:** `201 Created` for each

---

### 5️⃣ View All Products

**Endpoint:** `GET /api/products`

**Expected Response:** `200 OK`
```json
[
  {
    "productId": 1,
    "name": "Dell XPS 15 Laptop",
    "price": 1500.00,
    "stock": 20
  },
  {
    "productId": 2,
    "name": "Logitech MX Master Mouse",
    "price": 99.99,
    "stock": 100
  },
  {
    "productId": 3,
    "name": "Mechanical Keyboard",
    "price": 150.00,
    "stock": 50
  }
]
```

---

### 6️⃣ Register a Customer User

**Endpoint:** `POST /api/users/register`

**Request Body:**
```json
{
  "username": "customer@test.com",
  "password": "Customer123!",
  "role": "Customer"
}
```

---

### 7️⃣ Create a Customer Profile

**Endpoint:** `POST /api/customers`

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

**Expected Response:** `201 Created`
```json
{
  "customerId": 1,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "orders": []
}
```

---

### 8️⃣ Create an Order (Test Discount - Under $100)

**Endpoint:** `POST /api/orders`

**Request Body:**
```json
{
  "customerId": 1,
  "paymentMethod": "Credit Card",
  "items": [
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

**Expected Response:** `201 Created`
```json
{
  "orderId": 1,
  "customerId": 1,
  "orderDate": "2024-02-14T10:30:00Z",
  "totalAmount": 99.99,  // No discount applied (under $100)
  "paymentMethod": "Credit Card",
  "status": "Pending",
  "orderItems": [...]
}
```

---

### 9️⃣ Create an Order (Test 5% Discount - Over $100)

**Endpoint:** `POST /api/orders`

**Request Body:**
```json
{
  "customerId": 1,
  "paymentMethod": "PayPal",
  "items": [
    {
      "productId": 2,
      "quantity": 1
    },
    {
      "productId": 3,
      "quantity": 1
    }
  ]
}
```

**Calculation:**
- Mouse: $99.99
- Keyboard: $150.00
- **Subtotal: $249.99**
- Discount: 5% (over $100 but under $200... wait, this is over $200!)
- **Actually gets 10% discount!**
- **Final Total: $224.99** (10% off)

---

### 🔟 Create an Order (Test 10% Discount - Over $200)

**Endpoint:** `POST /api/orders`

**Request Body:**
```json
{
  "customerId": 1,
  "paymentMethod": "Credit Card",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    }
  ]
}
```

**Calculation:**
- Laptop: $1500.00 × 2
- **Subtotal: $3000.00**
- Discount: 10% (over $200)
- **Final Total: $2700.00** ✅

**Expected Response:** `201 Created`

---

### 1️⃣1️⃣ View Customer Orders

**Endpoint:** `GET /api/customers/1/orders`

**Expected Response:** `200 OK` - Shows all 3 orders with discounts applied

---

### 1️⃣2️⃣ View All Orders (Admin Only)

**Endpoint:** `GET /api/orders`

**Expected Response:** `200 OK` - Shows all orders in the system

---

### 1️⃣3️⃣ Update Order Status (Admin Only)

**Endpoint:** `PUT /api/orders/1/status`

**Request Body:**
```json
{
  "status": "Shipped"
}
```

**Expected Response:** `204 No Content`

**📧 Check the console logs** - You should see an email notification logged:
```
Email notification sent to john.doe@example.com - Order #1 status changed to Shipped
```

---

### 1️⃣4️⃣ View Invoices (Admin Only)

**Endpoint:** `GET /api/invoices`

**Expected Response:** `200 OK`
```json
[
  {
    "invoiceId": 1,
    "orderId": 1,
    "invoiceDate": "2024-02-14T10:30:00Z",
    "totalAmount": 99.99
  },
  {
    "invoiceId": 2,
    "orderId": 2,
    "invoiceDate": "2024-02-14T10:35:00Z",
    "totalAmount": 224.99
  },
  {
    "invoiceId": 3,
    "orderId": 3,
    "invoiceDate": "2024-02-14T10:40:00Z",
    "totalAmount": 2700.00
  }
]
```

---

## 🧪 Test Edge Cases

### Test 1: Insufficient Stock

**Endpoint:** `POST /api/orders`

```json
{
  "customerId": 1,
  "paymentMethod": "Credit Card",
  "items": [
    {
      "productId": 1,
      "quantity": 999
    }
  ]
}
```

**Expected Response:** `400 Bad Request`
```json
{
  "errors": [
    "Order.InsufficientStock",
    "Insufficient stock for the requested quantity"
  ]
}
```

---

### Test 2: Non-Existent Customer

**Endpoint:** `POST /api/orders`

```json
{
  "customerId": 999,
  "paymentMethod": "Credit Card",
  "items": [
    {
      "productId": 1,
      "quantity": 1
    }
  ]
}
```

**Expected Response:** `404 Not Found`

---

### Test 3: Customer Trying to Access Admin Endpoint

**Logout and login as customer:**

```json
{
  "username": "customer@test.com",
  "password": "Customer123!"
}
```

**Try:** `GET /api/orders` (Get all orders)

**Expected Response:** `403 Forbidden`

---

## ✅ Verification Checklist

After completing the test scenario, verify:

- [x] Admin can create products
- [x] Admin can view all products
- [x] Customer profile can be created
- [x] Orders with different totals apply correct discounts:
  - [x] Under $100: No discount
  - [x] $100-$200: 5% discount
  - [x] Over $200: 10% discount
- [x] Product stock is updated after order creation
- [x] Invoices are automatically generated
- [x] Order status can be updated (admin only)
- [x] Email notifications are logged on status change
- [x] Role-based access control works:
  - [x] Admin can access all endpoints
  - [x] Customer cannot access admin-only endpoints
- [x] Validation works for invalid requests
- [x] Error handling returns proper error messages

---

## 🧪 Run Unit Tests

To verify the business logic:

```bash
cd OrderManagementSystem.Tests
dotnet test --verbosity detailed
```

**Expected Output:**
```
Test summary: total: 20, failed: 0, succeeded: 20, skipped: 0
```

---

## 🎯 What Was Tested?

✅ JWT Authentication & Authorization  
✅ User Registration & Login  
✅ Product CRUD Operations  
✅ Customer Management  
✅ Order Creation with Validation  
✅ Tiered Discount Calculation  
✅ Inventory Management  
✅ Invoice Generation  
✅ Order Status Updates  
✅ Email Notifications  
✅ Role-Based Access Control  
✅ Error Handling  
✅ Input Validation  

---

## 📚 Additional Resources

- **Full API Documentation**: See `OrderManagementSystem/README.md`
- **Implementation Summary**: See `IMPLEMENTATION_SUMMARY.md`
- **Swagger UI**: `https://localhost:5001/swagger`

---

## 🆘 Troubleshooting

### Issue: "Unauthorized" error
**Solution:** Make sure you clicked "Authorize" and entered `Bearer {token}`

### Issue: Can't access admin endpoints
**Solution:** Login with admin@test.com (or a user with "Admin" role)

### Issue: Build errors
**Solution:** Run `dotnet restore` and `dotnet build`

### Issue: Port already in use
**Solution:** Change the port in `launchSettings.json` or kill the process using port 5001

---

**Happy Testing! 🎉**
