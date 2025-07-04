﻿using Api.Common;
using Api.Data;
using Api.Model;
using Api.ModelDto;
using Microsoft.EntityFrameworkCore;

namespace Api.Service
{
    public class OrdersService
    {
        private readonly AppDbContext appDbContext;

        public OrdersService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDto orderHeaderCreateDto)
        {
            var order = new OrderHeader
            {
                AppUserId = orderHeaderCreateDto.AppUserId,
                CustomerName = orderHeaderCreateDto.CustomerName,
                CustomerEmail = orderHeaderCreateDto.CustomerEmail,
                OrderTotalAmount = orderHeaderCreateDto.OrderTotalAmount,
                TotalCount = orderHeaderCreateDto.TotalCount,
                OrderDateTime = DateTime.UtcNow,
                Status = string.IsNullOrEmpty(orderHeaderCreateDto.Status)
                    ? SharedData.OrderStatus.Pending
                    : orderHeaderCreateDto.Status
            };

            await appDbContext.OrderHeaders.AddAsync(order);
            await appDbContext.SaveChangesAsync();

            foreach (var orderDetailsDto in orderHeaderCreateDto.OrderDetailsDto)
            {
                var orderDetails = new OrderDetails
                {
                    OrderHeaderId = order.OrderHeaderId,
                    ProductId = orderDetailsDto.ProductId,
                    Quantity = orderDetailsDto.Quantity,
                    ItemName = orderDetailsDto.ItemName,
                    Price = orderDetailsDto.Price
                };

                await appDbContext.OrderDetails.AddAsync(orderDetails);
            }

            await appDbContext.SaveChangesAsync();
            return order;
        }

        public async Task<OrderHeader?> GetOrderByIdAsync(int id)
        {
            return await appDbContext
                .OrderHeaders
                .Include(items => items.OrderDetailItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(u => u.OrderHeaderId == id);
        }

        public async Task<IEnumerable<OrderHeader>> GetOrderByUserIdAsync(string userId)
        {
            var querry = appDbContext
                .OrderHeaders
                .Include(items => items.OrderDetailItems)
                .ThenInclude(x => x.Product)
                .OrderByDescending(u => u.AppUserId);

            if (!string.IsNullOrEmpty(userId))
            {
                return await querry
                    .Where(u => u.AppUserId == userId)
                    .ToListAsync();
            }

            return await querry.ToListAsync();
        }

        public async Task<bool> UpdateOrderHeaderAsync(int id, OrderHeaderUpdateDto orderHeaderUpdateDto)
        {
            if (orderHeaderUpdateDto == null ||
                orderHeaderUpdateDto.OrderHeaderId != id)
            {
                return false;
            }

            var orderHeader = await appDbContext
                .OrderHeaders
                .FirstOrDefaultAsync(oh => oh.OrderHeaderId == id);

            if (orderHeader == null)
            { 
                return false;
            }

            orderHeader.CustomerEmail = string.IsNullOrEmpty(orderHeaderUpdateDto.CustomerEmail)
                ? orderHeader.CustomerEmail : orderHeaderUpdateDto.CustomerEmail;

            orderHeader.CustomerName = string.IsNullOrEmpty(orderHeaderUpdateDto.CustomerName)
                ? orderHeader.CustomerName : orderHeaderUpdateDto.CustomerName;
            
            orderHeader.Status = string.IsNullOrEmpty(orderHeaderUpdateDto.Status)
                ? orderHeader.Status : orderHeaderUpdateDto.Status;

            await appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
