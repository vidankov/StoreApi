using Api.Data;
using Api.Model;
using Api.ModelDto;
using Api.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class OrderController : StoreController
    {
        private readonly OrdersService ordersService;

        public OrderController(AppDbContext dbContext, OrdersService ordersService) : base(dbContext)
        {
            this.ordersService = ordersService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseServer>> CreateOrder(
            [FromBody] OrderHeaderCreateDto orderHeaderCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Некорректное состояние модели заказа" }
                });
            }

            try
            {
                var order = await ordersService.CreateOrderAsync(orderHeaderCreateDto);
                // order.OrderDetailItems = null;

                return Ok(new ResponseServer
                {
                    StatusCode = HttpStatusCode.Created,
                    Result = order
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Ошибка при создании заказа", ex.Message }
                });
            }
        }
    }
}
