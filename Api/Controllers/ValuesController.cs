using Api.Data;
using Api.Service;

namespace Api.Controllers
{
    public class ValuesController : StoreController
    {
        private readonly OrdersService ordersService;

        public ValuesController(AppDbContext dbContext, OrdersService ordersService) : base(dbContext)
        {
            this.ordersService = ordersService;
        }
    }
}
