using Api.Service;

namespace Api.Extensions
{
    public static class OrdersServiceExtension
    {
        public static IServiceCollection AddOrdersService(
            this IServiceCollection services)
        {
            return services.AddScoped<OrdersService>();
        }
    }
}
