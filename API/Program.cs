using API.Extensions;
using API.Filters;
using API.Hubs;
using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddApplicationServices(config);
builder.Services.AddIdentityServices(config);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
}).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(policy => policy.AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200").AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<BookingHub>("hubs/booking");
});

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var db = services.GetRequiredService<DataContext>();
var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();
await Seed.SeedUsers(userManager, roleManager);
await Seed.SeedFloorsAndWorkPlaces(db);

app.Run();
