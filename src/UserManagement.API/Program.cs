using Microsoft.EntityFrameworkCore;
using UserManagement.API.Endpoints;
using UserManagement.API.Infrastructure;
using UserManagement.API.Validators;
using UserManagement.Domain.Interfaces;
using UserManagement.Infra.Context;
using UserManagement.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Adiciona o contexto da base de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddUserValidations();

builder.Services.AddEndpoints();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.DocumentTitle = "User Management API - Swagger UI";
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapEndpoints();


app.Run();
