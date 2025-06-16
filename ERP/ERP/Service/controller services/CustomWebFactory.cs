using ERP.Model;
using ERP.Program;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

public class CustomWebAppFactory : WebApplicationFactory<ApplicationMain>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            // connect to postgres
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost:port=5432;Database=ERP_Test;Username=erpuser;Password=erppassword");
            });
        });
    }
}
