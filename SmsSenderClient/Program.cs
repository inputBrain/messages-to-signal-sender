using Microsoft.EntityFrameworkCore;
using SmsSenderClient.Configs;
using SmsSenderClient.Database;
using SmsSenderClient.Database.Message;
using SmsSenderClient.Middleware;
using SmsSenderClient.Services;

var builder = WebApplication.CreateBuilder(args);

var typeOfContent = typeof(Program);
builder.Services.AddDbContext<PostgreSqlContext>(
    opt => opt.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSqlConnection"),
        b => b.MigrationsAssembly(typeOfContent.Assembly.GetName().Name)
    )
);

builder.Services.AddControllersWithViews();

builder.Services.Configure<SignalSettings>(builder.Configuration.GetSection("Signal"));
builder.Services.AddHttpClient<ISignalSender, SignalSender>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();