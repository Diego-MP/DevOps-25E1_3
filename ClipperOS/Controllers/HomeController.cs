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
            string sql = "SELECT name, login FROM users ORDER BY name";
            
            var users = await _dbConnect.ExecuteReaderAsync<UserModel>(
                sql,
                reader => new UserModel
                {
                    Name = reader.GetString(0),
                    Login = reader.GetString(1)
                }
            );
            
            var viewModel = new HomeIndexViewModel
            {
                Users = users,
                TotalUsers = users.Count,
                LastUpdated = DateTime.Now
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários");
            
            var viewModel = new HomeIndexViewModel
            {
                Users = new List<UserModel>(),
                ErrorMessage = "Não foi possível carregar os dados de usuários.",
                HasError = true
            };
            
            return View(viewModel);
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
