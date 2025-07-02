using Api.Data;
using Api.Model;
using Api.Service.Payment;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class PaymentController : StoreController
    {
        private readonly IPaymentService paymentService;

        public PaymentController(AppDbContext dbContext,
            IPaymentService paymentService) : base(dbContext)
        {
            this.paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseServer>> MakePayment(
            string userId, int orderHeaderId, string cardNumber)
        {
            try
            {
                return await paymentService.HandlePaymentAsync(userId, orderHeaderId, cardNumber);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "При обработке платежа произошла ошибка", ex.Message }
                });
            }
        }
    }
}
