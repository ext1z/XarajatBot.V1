using AutoTest.Services;
using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Options;
using Xarajat.Bot.Repositories;
using Xarajat.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<XarajatDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("XarajatBotDb"));
    options.UseLazyLoadingProxies();
});

builder.Services.Configure<XarajatBotOptions>(
    builder.Configuration.GetSection(nameof(XarajatBotOptions)));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<OutlayRepository>();
builder.Services.AddScoped<TelegramBotService>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
