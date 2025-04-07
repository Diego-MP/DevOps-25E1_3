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
        
        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var sampleProducts = new List<ProductModel>
            {
                new ProductModel { Id = Guid.NewGuid(), Name = "Produto 1" },
                new ProductModel { Id = Guid.NewGuid(), Name = "Produto 2" }
            };

            mockRepo.Setup(repo => repo.GetAllProducts())
                    .ReturnsAsync(sampleProducts);

            var controller = new ProductsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductModel>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
        }

        [Fact]
        public async Task DeleteProduct_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var productId = Guid.NewGuid().ToString();

            mockRepo.Setup(r => r.GetProductById(productId))
                    .ReturnsAsync(new ProductModel { Id = Guid.Parse(productId) });

            mockRepo.Setup(r => r.DeleteProduct(productId)).Returns(Task.CompletedTask);

            var controller = new ProductsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            using var doc = JsonDocument.Parse(json);
            var message = doc.RootElement.GetProperty("Message").GetString();

            Assert.Contains("deletado com sucesso", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var productId = Guid.NewGuid().ToString();

            mockRepo.Setup(r => r.GetProductById(productId))
                    .ReturnsAsync((ProductModel)null);

            var controller = new ProductsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteProduct(productId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var json = JsonSerializer.Serialize(notFound.Value);
            using var doc = JsonDocument.Parse(json);
            var message = doc.RootElement.GetProperty("Message").GetString();

            Assert.Contains("não encontrado", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var invalidId = "not-a-guid";

            var controller = new ProductsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteProduct(invalidId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonSerializer.Serialize(badRequest.Value);
            using var doc = JsonDocument.Parse(json);
            var message = doc.RootElement.GetProperty("Message").GetString();

            Assert.Contains("inválido", message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
