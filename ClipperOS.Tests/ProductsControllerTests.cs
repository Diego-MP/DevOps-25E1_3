using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using ClipperOS.Controllers;
using ClipperOS.Models;
using ClipperOS.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace ClipperOS.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task AddProduct_ReturnsOkResult_WhenProductIsValid()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var productToAdd = new ProductModel
            {
                Name = "Produto Teste",
                Brand = "Marca X",
                Model = "Modelo Y",
                Category = "Categoria Z",
                Price = 100.50M,
                CodeBar = "7891234567890",
                Stock = 5
            };

            // Simula o retorno do repositório ao adicionar produto
            mockRepo.Setup(r => r.AddProduct(It.IsAny<ProductModel>()))
                .ReturnsAsync((ProductModel p) =>
                {
                    p.Id = Guid.NewGuid();
                    p.Created = DateTime.UtcNow;
                    return p;
                });

            var controller = new ProductsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.AddProduct(productToAdd);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Serializa e desserializa o objeto para poder acessá-lo com segurança
            var json = JsonSerializer.Serialize(okResult.Value);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.Equal("Produto adicionado com sucesso!", root.GetProperty("Message").GetString());

            var product = root.GetProperty("Product");
            Assert.NotNull(product);
            Assert.Equal(productToAdd.Name, product.GetProperty("Name").GetString());
        }
    }
}
