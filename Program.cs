using Microsoft.EntityFrameworkCore;
using RabotaGovnoClone.Data;
using RabotaGovnoClone.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ReviewRepository>();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=app.db"));

var app = builder.Build();

// ПРИМЕНЕНИЕ МИГРАЦИЙ ПРИ ЗАПУСКЕ
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Применяет все ожидающие миграции к базе данных
        dbContext.Database.Migrate();
        Console.WriteLine("Миграции успешно применены");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при применении миграций: {ex.Message}");
        // Логируйте ошибку, но не останавливайте приложение
        // или остановите, если база данных критична
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<RabotaGovnoClone.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
