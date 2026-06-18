using Optivio.Domin.Models;
using Optivio.Domin.Models.Enums;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int userId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);
            if (order == null) return null;
            return MapToDto(order);
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Currency = dto.Currency,
                Discount = dto.Discount,
                ShippingCost = dto.ShippingCost,
                EstimatedDelivery = dto.EstimatedDelivery,
                TotalsPrice = dto.Items.Sum(i => i.Price * i.Quantity) - dto.Discount + dto.ShippingCost,
                OrderItems = dto.Items.Select((item, index) => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ItemNumber = index + 1,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(order);
        }

        private OrderDto MapToDto(Order order) => new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            Currency = order.Currency,
            Discount = order.Discount,
            ShippingCost = order.ShippingCost,
            TotalsPrice = order.TotalsPrice,
            EstimatedDelivery = order.EstimatedDelivery,
            Items = order.OrderItems.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                ThumbnailUrl = i.Product?.ThumbnailUrl ?? "",
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto);
        }

        public async Task UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            var order = await _orderRepository.GetByIdForAdminAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            order.Status = Enum.Parse<OrderStatus>(dto.Status);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<OrderDto> ApplyCouponAsync(int userId, int orderId, ApplyCouponDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);
            if (order == null)
                throw new Exception("Order not found");

            // Coupon Logic بسيطة
            decimal discountAmount = dto.CouponCode switch
            {
                "SAVE10" => order.TotalsPrice * 0.10m,
                "SAVE20" => order.TotalsPrice * 0.20m,
                _ => throw new Exception("Invalid coupon code")
            };

            order.CouponCode = dto.CouponCode;
            order.Discount = discountAmount;
            order.TotalsPrice -= discountAmount;

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task CancelOrderAsync(int userId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                throw new Exception("Cannot cancel an order that has already been shipped or delivered.");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Order is already cancelled.");

            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
