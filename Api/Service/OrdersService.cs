using Api.Data;

namespace Api.Service
{
    public class OrdersService
    {
        private readonly AppDbContext appDbContext;

        public OrdersService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
    }
}
