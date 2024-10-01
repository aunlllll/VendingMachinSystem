using VendingMachinSystem.Models;
using VendingMachinSystem.ViewModels;

namespace VendingMachinSystem.Interface
{
    public interface IProductService
    {
        Task<ProductListViewModel> GetProductsAsync(string sortColumn, bool isSortDescending, string searchTerm, int currentPage, int pageSize);
        Task<PurchaseListViewModel> GetPurchaseListAsync(string sortColumn, bool isSortDescending, string searchTerm, int currentPage, int pageSize);

        Task<bool> CreateProductAsync(Product product);

        Task<Product> GetProductByIdAsync(int id);

        Task<bool> UpdateProductAsync(Product product);

        Task<bool> DeleteProductAsync(int id);

        Task<bool> CompletePurchaseAsync(int productId, int quantity, string userId);
    }
}
