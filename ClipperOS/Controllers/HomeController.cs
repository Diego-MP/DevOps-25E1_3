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
    
    [HttpGet]
    public IActionResult NewProduct()
    {
        return View(new ProductModel()); // Passe um modelo vazio
    }

    [HttpPost]
    public async Task<IActionResult> NewProduct(ProductModel model)
    {
        if (!ModelState.IsValid)
        {
            // Log os erros para depuração
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(model); // Retorna a view com os erros de validação
        }

        Console.WriteLine($"Name: {model.Name}");
        Console.WriteLine($"Price: {model.Price}");
        Console.WriteLine($"CodeBar: {model.CodeBar}");

        try
        {
            var repo = new ProductRepository(_dbConnect);
            await repo.AddProduct(model);
            TempData["Success"] = "Produto adicionado com sucesso!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar produto");
            ModelState.AddModelError("", "Erro ao salvar produto.");
            return View(model);
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
