using ClipperOS.Infrastructure;
using ClipperOS.Repositories; // <-- Adicione isso

var builder = WebApplication.CreateBuilder(args);

// Adiciona MVC
builder.Services.AddControllersWithViews();

// Registra DbConnect
builder.Services.AddSingleton<DbConnect>(sp => {
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection") 
                           ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
    return new DbConnect(connectionString);
});

// ✅ Registra a interface e sua implementação
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets(); // Mantido seu método personalizado

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets(); // Mantido seu método de extensão personalizado

app.Run();