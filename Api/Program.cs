using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPostgreSqlDbContext(builder.Configuration);
builder.Services.AddPostgreSqlIdentityContext();
builder.Services.AddJwtTokenGenerator();

builder.Services.AddConfigureIdentityOptions();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    // добавляет middleware для Swagger UI - веб-интерфейса, который позволяет просматривать и тестировать ваше API
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.Services.InitializeRoleAsync();

app.Run();
