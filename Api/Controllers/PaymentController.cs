using Api.Data;
using Api.Model;
using Api.ModelDto;
using Api.Service.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Controllers
{
    public class PaymentController : StoreController
    {
        private readonly IPaymentService paymentService;

        public PaymentController(AppDbContext dbContext, IPaymentService paymentService) : base(dbContext)
        {
            this.paymentService = paymentService;
        }


    }
}
