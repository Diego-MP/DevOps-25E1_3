using Microsoft.AspNetCore.Mvc;
using ClipperOS.Models;
using ClipperOS.Infrastructure;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClipperOS.Controllers
{
    [Route("api/[controller]")] // Rota base: /api/products
    public class ProductsController : ControllerBase
    {
        private readonly DbConnect _dbConnect;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(DbConnect dbConnect, ILogger<ProductsController> logger)
        {
            _dbConnect = dbConnect;
            _logger = logger;
        }

        // POST: /api/products - Adicionar um produto
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel model)
        {
            // Remover validação de Id e Created, pois são gerados pelo banco
            if (ModelState.ContainsKey("Id"))
            {
                ModelState.Remove("Id");
            }
            if (ModelState.ContainsKey("Created"))
            {
                ModelState.Remove("Created");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var repo = new ProductRepository(_dbConnect);
                var addedProduct = await repo.AddProduct(model);
                return Ok(new { Message = "Produto adicionado com sucesso!", Product = addedProduct });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar produto");
                return StatusCode(500, new { Message = "Erro ao salvar produto.", Details = ex.Message });
            }
        }

        // GET: /api/products - Listar todos os produtos
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var repo = new ProductRepository(_dbConnect);
                var products = await repo.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar produtos");
                return StatusCode(500, new { Message = "Erro ao listar produtos.", Details = ex.Message });
            }
        }

        // DELETE: /api/products/{id} - Deletar um produto por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid guidId))
                {
                    return BadRequest(new { Message = "ID inválido. Deve ser um UUID válido." });
                }

                var repo = new ProductRepository(_dbConnect);
                var productExists = await repo.GetProductById(id);
                if (productExists == null)
                {
                    return NotFound(new { Message = $"Produto com ID {id} não encontrado." });
                }

                await repo.DeleteProduct(id);
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