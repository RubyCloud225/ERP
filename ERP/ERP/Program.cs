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


public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Program>();
            })
            .Build()
            .Run();
    }
    public Program(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                policyBuilder =>
                {
                    policyBuilder.WithOrigins("https://MyFrontend.com") // Allow requests from my frontend origin only
                        .AllowCredentials()
                        .AllowAnyMethod() // Allow any HTTP method
                        .AllowAnyHeader(); // Allow headers
                });
        });
        services.AddHttpClient<IDocumentService, DocumentService>();
        services.AddHttpClient<IDocumentProcessor, DocumentProcessor>();
        services.AddHttpClient<ILlmService, LlmService>();
        services.AddHttpClient<ICloudStorageService, CloudStorageService>();
        services.AddHttpClient<IPurchaseInvoiceService, PurchaseInvoiceService>();
        services.AddHttpClient<ISalesInvoiceService, SalesInvoiceService>();
        services.AddHttpClient<IUserService, UserService>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = $"{Configuration["Redis__Host"]}";
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
