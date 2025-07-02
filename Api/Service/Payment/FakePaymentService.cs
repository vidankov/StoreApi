using Api.Common;
using Api.Data;
using Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Service.Payment
{
    public class FakePaymentService : IPaymentService
    {
        private readonly AppDbContext appDbContext;

        public FakePaymentService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<ActionResult<ResponseServer>> HandlePaymentAsync(string userId, int orderHeaderId, string cardNumber)
        {
            var cartFromDb = await appDbContext
                .ShoppingCarts
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItems => cartItems.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

            if (cartFromDb == null
                || cartFromDb.CartItems == null
                || cartFromDb.CartItems.Count == 0)
            {
                return new BadRequestObjectResult(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Корзина пуста или не найдена" }
                });
            }

            cartFromDb.TotalAmount = cartFromDb
                .CartItems
                .Sum(item => item.Quantity * item.Product.Price);

            var orderHeaderFromDb = await appDbContext
                .OrderHeaders
                .FirstOrDefaultAsync(oh => oh.OrderHeaderId == orderHeaderId);

            if (orderHeaderFromDb == null)
            {
                return new BadRequestObjectResult(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Такого заказа не существует" }
                });
            }

            await Task.Delay(5000);
            PaymentResponse paymentResponse = new();

            if (cardNumber.Equals("1111 2222 3333 4444"))
            {
                paymentResponse.Success = true;
                paymentResponse.Secret = "какой-то_секретный_токен";
                paymentResponse.IntentId = $"оплата по заказу №{orderHeaderFromDb.OrderHeaderId}";
            }
            else
            {
                paymentResponse.Success = false;
                paymentResponse.Secret = string.Empty;
                paymentResponse.IntentId = string.Empty;
                paymentResponse.ErrorMessage = "ошибка оплаты";
            }

            if (!paymentResponse.Success)
            {
                return new BadRequestObjectResult(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { paymentResponse.ErrorMessage },
                    Result = paymentResponse
                });
            }

            orderHeaderFromDb.Status = SharedData.OrderStatus.ReadyToShip;
            await appDbContext.SaveChangesAsync();

            return new OkObjectResult(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = orderHeaderFromDb
            });
        }
    }
}
