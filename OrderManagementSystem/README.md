# Order Management System API

A comprehensive ASP.NET Core Web API for managing orders, customers, products, and invoices with JWT authentication and role-based access control.

## Features

- **JWT Authentication**: Secure endpoints with JWT tokens
- **Role-Based Access Control (RBAC)**: Admin and Customer roles
- **Order Management**: Create, view, and update orders
- **Customer Management**: Create customers and view order history
- **Product Management**: CRUD operations for products (admin only)
- **Invoice Generation**: Automatic invoice generation on order creation
- **Tiered Discounts**: 5% off for orders over $100, 10% off for orders over $200
- **Inventory Management**: Automatic stock updates
- **Email Notifications**: Email notifications on order status changes
- **Input Validation**: FluentValidation for all requests
- **Global Exception Handling**: Consistent error responses
- **Swagger Documentation**: Interactive API documentation

## Tech Stack

- .NET 10
- Entity Framework Core (In-Memory Database)
- JWT Bearer Authentication
- FluentValidation
- Swagger/OpenAPI

## Getting Started

### Prerequisites

- .NET 10 SDK

### Running the Application

```bash
dotnet run --project OrderManagementSystem
```

The API will be available at `https://localhost:5001` (or the port specified in launchSettings.json)

### Swagger UI

Navigate to `https://localhost:5001/swagger` to access the interactive API documentation.

## API Endpoints

### Authentication Endpoints

#### Register a New User
```
POST /api/users/register
```
**Request Body:**
```json
{
  "username": "john.doe@example.com",
  "password": "SecurePassword123!",
  "role": "Customer"
}
```

#### Login
```
POST /api/users/login
```
**Request Body:**
```json
{
  "username": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```
**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "role": "Customer",
  "userId": 1
}
```

### Customer Endpoints

#### Create a Customer
```
POST /api/customers
Authorization: Bearer {token}
```
**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

#### Get Customer Orders
```
GET /api/customers/{customerId}/orders
Authorization: Bearer {token}
```

### Product Endpoints

#### Get All Products
```
GET /api/products
```

#### Get Product by ID
```
GET /api/products/{productId}
```

#### Create a Product (Admin Only)
```
POST /api/products
Authorization: Bearer {token}
```
**Request Body:**
```json
{
  "name": "Laptop",
  "price": 999.99,
  "stock": 50
}
```

#### Update a Product (Admin Only)
```
PUT /api/products/{productId}
Authorization: Bearer {token}
```
**Request Body:**
```json
{
  "name": "Updated Laptop",
  "price": 899.99,
  "stock": 45
}
```

### Order Endpoints

#### Create an Order
```
POST /api/orders
Authorization: Bearer {token}
```
**Request Body:**
```json
{
  "customerId": 1,
  "paymentMethod": "Credit Card",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

#### Get Order by ID
```
GET /api/orders/{orderId}
Authorization: Bearer {token}
```

#### Get All Orders (Admin Only)
```
GET /api/orders
Authorization: Bearer {token}
```

#### Update Order Status (Admin Only)
```
PUT /api/orders/{orderId}/status
Authorization: Bearer {token}
```
**Request Body:**
```json
{
  "status": "Shipped"
}
```

### Invoice Endpoints (Admin Only)

#### Get Invoice by ID
```
GET /api/invoices/{invoiceId}
Authorization: Bearer {token}
```

#### Get All Invoices
```
GET /api/invoices
Authorization: Bearer {token}
```

## Business Logic

### Tiered Discounts
- Orders over $100: 5% discount
- Orders over $200: 10% discount

### Order Validation
- Validates product existence
- Validates sufficient stock before order placement
- Automatic stock deduction on order creation

### Invoice Generation
- Invoices are automatically generated when orders are created
- Invoice contains order details and total amount

### Email Notifications
- Email notifications are sent to customers when order status changes
- Currently logs to console (can be configured with real email service)

## Testing with Swagger

1. Start the application
2. Navigate to `/swagger`
3. Register a new user with "Admin" role:
   ```json
   {
     "username": "admin@example.com",
     "password": "Admin123!",
     "role": "Admin"
   }
   ```
4. Login to get JWT token
5. Click "Authorize" button in Swagger UI
6. Enter: `Bearer {your-token-here}`
7. Test the endpoints

## Sample Test Flow

1. **Register Admin User** → POST /api/users/register
2. **Login as Admin** → POST /api/users/login (get token)
3. **Create Products** → POST /api/products
4. **Register Customer** → POST /api/users/register
5. **Create Customer** → POST /api/customers
6. **Create Order** → POST /api/orders
7. **View Order** → GET /api/orders/{id}
8. **Update Order Status** → PUT /api/orders/{id}/status (Admin only)
9. **View Invoice** → GET /api/invoices/{id} (Admin only)
10. **View Customer Orders** → GET /api/customers/{id}/orders

## Configuration

### JWT Settings (appsettings.json)
```json
{
  "Jwt": {
    "Key": "Your-Secret-Key-Here-Min-32-Characters",
    "Issuer": "OrderManagementSystem",
    "Audience": "OrderManagementSystemClient",
    "ExpiryMinutes": 240
  }
}
```

### CORS Configuration
Configure allowed origins in `appsettings.json`:
```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:4200"
  ]
}
```

## Error Handling

The API uses consistent error responses:
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
  "title": "Bad Request",
  "status": 400,
  "errors": [
    "Error.Code",
    "Error message description"
  ]
}
```

## Database

The application uses an in-memory database for simplicity. Data is reset on application restart.

To use a persistent database (SQL Server, PostgreSQL, etc.), update the `AddDatabase` method in `DependencyInjection.cs`:

```csharp
services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
```

## Future Enhancements

- Add unit tests for services and controllers
- Implement real email service integration (SendGrid, AWS SES)
- Add pagination for list endpoints
- Add order cancellation functionality
- Add refund processing
- Implement order tracking
- Add product categories
- Add customer addresses
- Implement shipping cost calculation
- Add order filtering and search capabilities
- Export invoices to PDF

## License

This project is for educational purposes.
