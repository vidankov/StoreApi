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

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseServer>> GetOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Неверный идентификатор заказа" }
                });
            }

            try
            {
                var orderHeader = await ordersService.GetOrderById(id);
                if (orderHeader == null)
                {
                    return NotFound(new ResponseServer
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessages = { "Заказ не найден" }
                    });
                }

                return Ok(new ResponseServer
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = orderHeader
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Ошибка при получении заказа", ex.Message }
                });
            }
        }
    }
}
