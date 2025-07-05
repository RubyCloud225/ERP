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

namespace ERP.Program;
public class ApplicationMain
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<ApplicationMain>();
            })
            .Build()
            .Run();
    }
    public ApplicationMain(IConfiguration configuration)
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
        // Removed non-existing services IDocumentProcessor, DocumentProcessor, IBankService, BankService
        services.AddHttpClient<ILlmService, LlmService>();
        services.AddHttpClient<ICloudStorageService, CloudStorageService>();
        services.AddHttpClient<IPurchaseInvoiceService, PurchaseInvoiceService>();
        services.AddHttpClient<ISalesInvoiceService, SalesInvoiceService>();
        services.AddHttpClient<IUserService, UserService>();

        services.AddScoped<IVATReturnService, VATReturnService>();

        services.AddScoped<IJournalEntryService, JournalEntryService>();

        services.AddSingleton<NominalLedgerService>();
        services.AddSingleton<IFRSBalanceSheetService>();

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
        app.UseMiddleware<UserDataFilterMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<PropertyValidationMiddleware>();
        app.UseMiddleware<PropertyCachingMiddleware>();
        app.UseMiddleware<PropertyLoggingMiddleware>();
        app.UseMiddleware<RateLimitingMiddleware>();
        app.UseMiddleware<FileValidationMiddleware>();


        app.UseCors("AllowSpecificOrigin");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
