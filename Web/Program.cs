using Business;
using Data;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();

builder.Services.AddScoped<PersonaData>();
builder.Services.AddScoped<PersonBusiness>();

builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<PermisoBusiness>();

builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormBusiness>();

builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusiness>();

builder.Services.AddScoped<RolFormPermissionData>();
builder.Services.AddScoped<RolFormPermissionBusiness>();

builder.Services.AddScoped<RolUserData>();
builder.Services.AddScoped<RolUserBusiness>();

builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserBusiness>();

builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<FormModuleBusiness>();

builder.Services.AddScoped<ReviewData>();
builder.Services.AddScoped<ReviewBusiness>();

builder.Services.AddScoped<ConsumerData>();
builder.Services.AddScoped<ConsumerBusiness>();

builder.Services .AddScoped<ProducerData>();
builder.Services.AddScoped<ProducerBusiness>();

builder.Services.AddScoped<ProductData>();
builder.Services.AddScoped<ProductBusiness>();

builder.Services.AddScoped<CategoryData>();
builder.Services.AddScoped<CategoryBusiness>();

builder.Services.AddScoped<FavoriteData>();
builder.Services.AddScoped<FavoriteBusiness>();

builder.Services.AddScoped<OrderData>();
builder.Services.AddScoped<OrderBusiness>();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
