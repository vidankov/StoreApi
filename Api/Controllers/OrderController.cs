﻿using Api.Data;
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
                var orderHeader = await ordersService.GetOrderByIdAsync(id);
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

        [HttpGet]
        public async Task<ActionResult<ResponseServer>> GetOrdersByUserId(string userId)
        {
            try
            {
                var orderHeaders = await ordersService.GetOrderByUserIdAsync(userId);
                return Ok(new ResponseServer
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = orderHeaders
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessages = { "При обработке возникла проблема", ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseServer>> UpdateOrderHeader(int id,
            [FromBody] OrderHeaderUpdateDto orderHeaderUpdateDto)
        {
            try
            {
                if (await ordersService.UpdateOrderHeaderAsync(id, orderHeaderUpdateDto))
                {
                    return Ok(new ResponseServer
                    {
                        StatusCode = HttpStatusCode.OK,
                        Result = "Заказ успешно обновлён!"
                    });
                }
                else
                {
                    return BadRequest(new ResponseServer
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessages = { "Ошибка обновления заказа!" }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessages = { "Что-то пошло не так", ex.Message }
                });
            }
        }
    }
}
