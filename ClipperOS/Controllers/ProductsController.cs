using Microsoft.AspNetCore.Mvc;
using ClipperOS.Models;
using ClipperOS.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ClipperOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel model)
        {
            if (ModelState.ContainsKey("Id")) ModelState.Remove("Id");
            if (ModelState.ContainsKey("Created")) ModelState.Remove("Created");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var addedProduct = await _repository.AddProduct(model);
                return Ok(new { Message = "Produto adicionado com sucesso!", Product = addedProduct });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar produto");
                return StatusCode(500, new { Message = "Erro ao salvar produto.", Details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _repository.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar produtos");
                return StatusCode(500, new { Message = "Erro ao listar produtos.", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid guidId))
                {
                    return BadRequest(new { Message = "ID inválido. Deve ser um UUID válido." });
                }

                var product = await _repository.GetProductById(id);
                if (product == null)
                {
                    return NotFound(new { Message = $"Produto com ID {id} não encontrado." });
                }

                await _repository.DeleteProduct(id);
                return Ok(new { Message = $"Produto com ID {id} deletado com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar produto");
                return StatusCode(500, new { Message = "Erro ao deletar produto.", Details = ex.Message });
            }
        }
    }
}
