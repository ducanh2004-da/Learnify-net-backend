using Learnify.Business.Interfaces;
using Learnify.Business.Services;
using Learnify.Repository.Data;
using Learnify.Repository.Interfaces;
using Learnify.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database: truy?n rõ ki?u AppDbContext cho AddDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection: truy?n rõ interface và implementation
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// CORS
// ? CORS cho phép NestJS g?i
builder.Services.AddCors(options =>
{
    options.AddPolicy("NestJSPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:10000") // NestJS port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NestJSPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
