using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Order;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Errors;

namespace OrderManagementSystem.Service;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ICustomerRepository customerRepository,
    IEmailService emailService) : IOrderService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result<OrderResponse>> CreateOrderAsync(OrderCreateRequest request)
    {
        var customerExists = await _customerRepository.ExistsAsync(request.CustomerId);

        if (!customerExists) return Result.Failure<OrderResponse>(CustomerErrors.NotFound);

        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);

            if (product is null)
                return Result.Failure<OrderResponse>(ProductErrors.NotFound);

            if (product.Stock < itemRequest.Quantity)
                return Result.Failure<OrderResponse>(OrderErrors.InsufficientStock);

            product.Stock -= itemRequest.Quantity;
            await _productRepository.UpdateAsync(product);

            var orderItem = new OrderItem
            {
                ProductId = product.ProductId,
                Quantity = itemRequest.Quantity,
                UnitPrice = product.Price,
                Discount = 0
            };

            orderItems.Add(orderItem);
            totalAmount += product.Price * itemRequest.Quantity;
        }

        if (totalAmount > 200)
            totalAmount *= 0.90m;
        else if (totalAmount > 100) totalAmount *= 0.95m;

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            PaymentMethod = request.PaymentMethod,
            Status = "Pending",
            OrderItems = orderItems,
            Invoice = new Invoice
            {
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = totalAmount
            }
        };

        await _orderRepository.AddAsync(order);

        var response = MapToOrderResponse(order);
        return Result.Success(response);
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        return Result.Success(MapToOrderResponse(order));
    }

    public async Task<Result<IEnumerable<OrderResponse>>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        var response = orders.Select(MapToOrderResponse);

        return Result.Success(response);
    }

    public async Task<Result> UpdateOrderStatusAsync(int orderId, string status)
    {
        // 1. Check if the order exists
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order is null) return Result.Failure(OrderErrors.NotFound);

        // 2. Get customer email for notification
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
        if (customer is not null)
        {
            // 3. Send email notification before updating status
            await _emailService.SendOrderStatusChangeEmailAsync(customer.Email, orderId, status);
        }

        await orderRepository.UpdateStatusAsync(orderId, status);

        return Result.Success();
    }

    private static OrderResponse MapToOrderResponse(Order order)
    {
        return new OrderResponse(
            order.OrderId,
            order.CustomerId,
            order.OrderDate,
            order.TotalAmount,
            order.PaymentMethod,
            order.Status,
            order.OrderItems.Select(oi => new OrderItemResponse(
                oi.ProductId,
                oi.Product?.Name ?? "Unknown Product",
                oi.Quantity,
                oi.UnitPrice,
                oi.Discount
            )).ToList()
        );
    }
}