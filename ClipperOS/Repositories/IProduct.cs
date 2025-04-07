using ClipperOS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClipperOS.Repositories
{
    public interface IProductRepository
    {
        Task<ProductModel> AddProduct(ProductModel product);
        Task<List<ProductModel>> GetAllProducts();
        Task<ProductModel> GetProductById(string id);
        Task DeleteProduct(string id);
    }
}