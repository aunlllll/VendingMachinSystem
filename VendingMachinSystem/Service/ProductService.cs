using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VendingMachinSystem.Data;
using VendingMachinSystem.Interface;
using VendingMachinSystem.Models;
using VendingMachinSystem.ViewModels;

namespace VendingMachinSystem.Service
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;            
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            _context.Add(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            _context.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> CompletePurchaseAsync(int productId, int quantity, string userId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.Quantity < quantity)
            {
                return false; // Indicate failure due to insufficient product
            }

            var transaction = new Transaction
            {
                ProductId = productId,
                UserId = userId,
                Quantity = quantity,
                TotalPrice = (double)(product.Price * quantity),
                PurchaseDate = DateTime.UtcNow
            };

            // Update product quantity
            product.Quantity -= quantity;

            using var transactionScope = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Transactions.Add(transaction);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                await transactionScope.CommitAsync();
                return true; // Purchase completed successfully
            }
            catch (Exception)
            {
                await transactionScope.RollbackAsync();
                return false; // Indicate failure
            }
        }
        public async Task<ProductListViewModel> GetProductsAsync(string sortColumn, bool isSortDescending, string searchTerm, int currentPage, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm));
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn)
                {
                    case "ProductName":
                        query = isSortDescending == true? query.OrderByDescending(p => p.ProductName):query.OrderBy(p => p.ProductName);
                        break;
                    case "Price":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                        break;
                    case "Quantity":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                        break;
                    case "CreatedDate":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate);
                        break;
                    case "UpdatedDate":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.UpdatedDate) : query.OrderBy(p => p.UpdatedDate);
                        break;
                    default:
                        query = query.OrderBy(p => p.Id);
                        break;
                }
            }

            // Pagination
            var totalCount = await query.CountAsync();
            var products = await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();


            return new ProductListViewModel
            {
                Products = products,
                TotalCount = totalCount,
                CurrentPage = currentPage,
                PageSize = pageSize,
                SortColumn = sortColumn,
                IsSortDescending = isSortDescending,
                SearchTerm = searchTerm
            };
        }

        public async Task<PurchaseListViewModel> GetPurchaseListAsync(string sortColumn, bool isSortDescending, string searchTerm, int currentPage, int pageSize)
        {
            var query = _context.Transactions.Include(x => x.Product).AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Product.ProductName.Contains(searchTerm));
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn)
                {
                    case "ProductName":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.Product.ProductName) : query.OrderBy(p => p.Product.ProductName);
                        break;
                    case "Price":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.Product.Price) : query.OrderBy(p => p.Product.Price);
                        break;
                    case "Quantity":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.Quantity) : query.OrderBy(p => p.Quantity);
                        break;
                    case "TotalPrice":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.TotalPrice) : query.OrderBy(p => p.TotalPrice);
                        break;
                    case "PurchaseDate":
                        query = isSortDescending == true ? query.OrderByDescending(p => p.PurchaseDate) : query.OrderBy(p => p.PurchaseDate);
                        break;
                    default:
                        query = query.OrderBy(p => p.Id);
                        break;
                }
            }

            // Pagination
            var totalCount = await query.CountAsync();
            var transactions = await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();


            return new PurchaseListViewModel
            {
                Transactions = transactions,
                TotalCount = totalCount,
                CurrentPage = currentPage,
                PageSize = pageSize,
                SortColumn = sortColumn,
                IsSortDescending = isSortDescending,
                SearchTerm = searchTerm
            };
        }

    }
}
