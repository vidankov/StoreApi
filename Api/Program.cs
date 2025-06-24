using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenCustomConfig();
builder.Services.AddPostgreSqlDbContext(builder.Configuration);
builder.Services.AddPostgreSqlIdentityContext();
builder.Services.AddJwtTokenGenerator();
builder.Services.AddConfigureIdentityOptions();
builder.Services.AddAuthenticationConfig(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddShoppingCartService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    // добавляет middleware для Swagger UI - веб-интерфейса, который позволяет просматривать и тестировать ваше API
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseCors(o =>
    o.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    .WithExposedHeaders("*")
    );
app.UseAuthentication();
app.UseAuthorization();

await app.Services.InitializeRoleAsync();

app.Run();
