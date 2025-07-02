using Api.Data;
using Api.Model;
using Api.ModelDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Controllers
{
    public class PaymentController : StoreController
    {
        public PaymentController(AppDbContext dbContext) : base(dbContext) { }


    }
}
