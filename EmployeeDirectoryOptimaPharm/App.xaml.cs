using EmployeeDirectoryOptimaPharm.Data;
using EmployeeDirectoryOptimaPharm.Models;
using EmployeeDirectoryOptimaPharm.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace EmployeeDirectoryOptimaPharm;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static ServiceProvider ServiceProvider { get; private set; }
    public static IConfiguration Configuration { get; private set; } 

    protected override void OnStartup(StartupEventArgs e)
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();

        base.OnStartup(e);

        EnsureDatabaseCreated();

        MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string connectionString = Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDBFulfillService, DBFulfillService>();
        services.AddScoped<IJsonDataService, JsonDataService>();

        services.AddTransient<MainWindowViewModel>();

        services.AddTransient<MainWindow>();
    }

    private void EnsureDatabaseCreated()
    {
        using(var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            bool databaseJustCreated = context.Database.EnsureCreated();
            context.Database.Migrate();
        }
    }
}

