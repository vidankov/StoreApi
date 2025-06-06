using Api.Data;
using Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Controllers
{
    public class ProductController : StoreController
    {
        public ProductController(AppDbContext dbContext) : base(dbContext) { }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = await dbContext.Products.ToListAsync()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = new ResponseServer();

            if (id <= 0)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Id должен быть больше нуля.");

                return BadRequest(response);
            }

            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Продукт с указанным id не найден.");

                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            response.Result = product;

            return Ok(response);
        }
    }
}
