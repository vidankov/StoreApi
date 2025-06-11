using Api.Data;
using Api.Model;
using Api.ModelDto;
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

        [HttpGet("{id}", Name = nameof(GetProductById))]
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

        [HttpPost]
        public async Task<ActionResult<ResponseServer>> CreateProduct(
            [FromBody] ProductCreateDto productCreateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (productCreateDto.Image is null ||
                        productCreateDto.Image.Length == 0)
                    {
                        return BadRequest(new ResponseServer()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            IsSuccess = false,
                            ErrorMessages = { "Image не может быть пустым" }
                        });
                    }
                    else
                    {
                        Product item = new()
                        {
                            Name = productCreateDto.Name,
                            Description = productCreateDto.Description,
                            SpecialTag = productCreateDto.SpecialTag,
                            Category = productCreateDto.Category,
                            Price = productCreateDto.Price,
                            Image = $"https://placehold.co/250"
                        };

                        await dbContext.Products.AddAsync(item);
                        await dbContext.SaveChangesAsync();

                        ResponseServer response = new()
                        {
                            StatusCode = HttpStatusCode.Created,
                            Result = item
                        };

                        return CreatedAtRoute(nameof(GetProductById), new { id = item.Id }, response);
                    }
                }
                else
                {
                    return BadRequest(new ResponseServer
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessages = { "Модель данных не подходит" }
                    });
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseServer
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Что-то поломалось", ex.Message}
                });
            }
        }
    }
}
