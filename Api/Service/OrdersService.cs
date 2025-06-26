using Api.Common;
using Api.Data;
using Api.Model;
using Api.ModelDto;

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
    }
}
