using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using ERP.Model;
using ERP.Middleware;
using ERP.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
                         {
                             options.AddPolicy("AllowSpecificOrigin",
                                               policyBuilder =>
                                               {
                                                   policyBuilder.WithOrigins("https://MyFrontend.com") //Allow requests from my frontend origin only
                                                       .AllowCredentials()
                                                       .AllowAnyMethod() // anyHTTP method
                                                       .AllowAnyHeader(); // Allow headers
                                               });
                         });
builder.Services.AddHttpClient<IDocumentService, DocumentService>();
builder.Services.AddHttpClient<IDocumentProcessor, DocumentProcessor>();
builder.Services.AddHttpClient<ILlmService, LlmService>();
builder.Services.AddHttpClient<ICloudStorageService, CloudStorageService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{builder.Configuration["Redis__Host"]}";
});

// Add Entity Framework Core services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//Remember these have to be inorder of what you want to do first
app.UseMiddleware<UserDataFilterMiddleware>();
app.UseMiddleware<PropertyValidationMiddleware>();
app.UseMiddleware<PropertyCachingMiddleware>();
app.UseMiddleware<PropertyLoggingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<FileValidationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
