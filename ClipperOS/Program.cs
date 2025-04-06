using ClipperOS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// builder.Services.AddSingleton<DbConnect>(sp => {
//     var configuration = sp.GetRequiredService<IConfiguration>();
//     var connectionString = configuration.GetConnectionString("DefaultConnection") 
//                            ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
//     return new DbConnect(connectionString);
// });

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