using ClipperOS.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClipperOS.Infrastructure
{
    public class ProductRepository
    {
        private readonly DbConnect _db;

        public ProductRepository(DbConnect db)
        {
            _db = db;
        }

        // Adicionar um produto e retornar o produto com Id e Created gerados
        public async Task<ProductModel> AddProduct(ProductModel product)
        {
            var sql = @"
                INSERT INTO products (name, brand, model, category, price, code_bar, stock)
                VALUES (@name, @brand, @model, @category, @price, @code_bar, @stock)
                RETURNING id, name, brand, model, category, price, code_bar, stock, created";

            var parameters = new[]
            {
                new NpgsqlParameter("@name", product.Name),
                new NpgsqlParameter("@brand", product.Brand),
                new NpgsqlParameter("@model", product.Model),
                new NpgsqlParameter("@category", product.Category),
                new NpgsqlParameter("@price", product.Price),
                new NpgsqlParameter("@code_bar", product.CodeBar),
                new NpgsqlParameter("@stock", product.Stock)
            };

            // Log para depuração (opcional)
            Console.WriteLine(sql);
            foreach (var p in parameters)
            {
                Console.WriteLine($"{p.ParameterName}: {p.Value}");
            }

            var addedProduct = await _db.ExecuteReaderAsync(sql, reader => new ProductModel
            {
                Id = reader.GetGuid(0), // Alterado de GetString para GetGuid
                Name = reader.GetString(1),
                Brand = reader.GetString(2),
                Model = reader.GetString(3),
                Category = reader.GetString(4),
                Price = reader.GetDecimal(5),
                CodeBar = reader.GetString(6),
                Stock = reader.GetInt32(7),
                Created = reader.GetDateTime(8)
            }, parameters);

            return addedProduct.FirstOrDefault();
        }

        // Listar todos os produtos
        public async Task<List<ProductModel>> GetAllProducts()
        {
            var sql = @"
                SELECT id, name, brand, model, category, price, code_bar, stock, created
                FROM products";

            return await _db.ExecuteReaderAsync(sql, reader => new ProductModel
            {
                Id = reader.GetGuid(0), // Alterado de GetString para GetGuid
                Name = reader.GetString(1),
                Brand = reader.GetString(2),
                Model = reader.GetString(3),
                Category = reader.GetString(4),
                Price = reader.GetDecimal(5),
                CodeBar = reader.GetString(6),
                Stock = reader.GetInt32(7),
                Created = reader.GetDateTime(8)
            });
        }

        // Obter um produto por ID
        public async Task<ProductModel> GetProductById(string id)
        {
            var sql = @"
                SELECT id, name, brand, model, category, price, code_bar, stock, created
                FROM products
                WHERE id = @id";

            var parameters = new[]
            {
                new NpgsqlParameter("@id", Guid.Parse(id)) // Converter string para Guid
            };

            var products = await _db.ExecuteReaderAsync(sql, reader => new ProductModel
            {
                Id = reader.GetGuid(0), // Alterado de GetString para GetGuid
                Name = reader.GetString(1),
                Brand = reader.GetString(2),
                Model = reader.GetString(3),
                Category = reader.GetString(4),
                Price = reader.GetDecimal(5),
                CodeBar = reader.GetString(6),
                Stock = reader.GetInt32(7),
                Created = reader.GetDateTime(8)
            }, parameters);

            return products.FirstOrDefault();
        }

        // Deletar um produto por ID
        public async Task DeleteProduct(string id)
        {
            var sql = @"
                DELETE FROM products
                WHERE id = @id";

            var parameters = new[]
            {
                new NpgsqlParameter("@id", Guid.Parse(id)) // Converter string para Guid
            };

            await _db.ExecuteNonQueryAsync(sql, parameters);
        }
    }
}