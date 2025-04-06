using ClipperOS.Models;
using Npgsql;
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

        public async Task AddProduct(ProductModel product)
        {
            var sql = @"
                INSERT INTO products (id, name, brand, model, category, price, codebar, stock, created)
                VALUES (@id, @name, @brand, @model, @category, @price, @codebar, @stock, @created)";

            var parameters = new[]
            {
                new NpgsqlParameter("@id", product.Id),
                new NpgsqlParameter("@name", product.Name),
                new NpgsqlParameter("@brand", product.Brand),
                new NpgsqlParameter("@model", product.Model),
                new NpgsqlParameter("@category", product.Category),
                new NpgsqlParameter("@price", product.Price),
                new NpgsqlParameter("@codebar", product.CodeBar),
                new NpgsqlParameter("@stock", product.Stock),
                new NpgsqlParameter("@created", product.Created),
            };

            await _db.ExecuteNonQueryAsync(sql, parameters);
        }
    }
}