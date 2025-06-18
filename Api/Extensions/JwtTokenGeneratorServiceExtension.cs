using Api.Service;

namespace Api.Extensions
{
    public static class JwtTokenGeneratorServiceExtension
    {
        public static IServiceCollection AddJwtTokenGenerator(
            this IServiceCollection services)
        {
            return services.AddScoped<JwtTokenGenerator>();
        }
    }
}
