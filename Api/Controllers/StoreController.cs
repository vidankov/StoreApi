using Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        protected readonly AppDbContext dbContext;

        public StoreController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
