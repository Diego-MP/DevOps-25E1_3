using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClipperOS.Models;
using ClipperOS.Infrastructure;

namespace ClipperOS.Controllers;

public class HomeController : Controller
{
    private readonly DbConnect _dbConnect;
    private readonly ILogger<HomeController> _logger;

    public HomeController(DbConnect dbConnect, ILogger<HomeController> logger)
    {
        _dbConnect = dbConnect;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            string sql = "SELECT id, name, brand, model, category, price, code_bar, stock, created FROM products ORDER BY created DESC";

            var products = await _dbConnect.ExecuteReaderAsync<ProductModel>(
                sql,
                reader => new ProductModel
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Brand = reader.GetString(2),
                    Model = reader.GetString(3),
                    Category = reader.GetString(4),
                    Price = reader.GetDecimal(5),
                    CodeBar = reader.GetString(6),
                    Stock = reader.GetInt32(7),
                    Created = reader.GetDateTime(8)
                }
            );

            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos");

            return View(new List<ProductModel>());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}
