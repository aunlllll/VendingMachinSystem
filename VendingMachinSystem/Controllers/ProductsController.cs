using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachinSystem.Interface;
using VendingMachinSystem.Models;

namespace VendingMachinSystem.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ProductsController(IProductService productService, SignInManager<IdentityUser> signInManager)
        {
            _productService = productService;
            _signInManager = signInManager;
        }

        // GET: products
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index(string sortColumn = "ProductName", bool isSortDescending = true, string searchTerm = "", int currentPage = 1, int pageSize = 5)
        {
            var viewModel = await _productService.GetProductsAsync(sortColumn, isSortDescending, searchTerm, currentPage, pageSize);
            return View(viewModel);
        }

        // GET: products/details/5
        [HttpGet("details/{id:int}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: products/create
        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: products/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,ProductName,Price,Quantity,CreatedDate,UpdatedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                var success = await _productService.CreateProductAsync(product);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(product);
        }

        // GET: products/edit/5
        [HttpGet("edit/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: products/edit/5
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,Price,Quantity,CreatedDate,UpdatedDate")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var success = await _productService.UpdateProductAsync(product);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(product);
        }

        // GET: products/delete/5
        [HttpGet("delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: products/delete/5
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // GET: products/purchase/5
        [HttpGet("purchase/{id:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Purchase(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: products/purchase/5
        [HttpPost("purchase/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Purchase(int id, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); ;
            if(userId == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            var success = await _productService.CompletePurchaseAsync(id, quantity, userId);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: products/purchaseList
        [HttpGet("purchaseList")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PurchaseList(string sortColumn = "ProductName", bool isSortDescending = true, string searchTerm = "", int currentPage = 1, int pageSize = 5)
        {
            var viewModel = await _productService.GetPurchaseListAsync(sortColumn, isSortDescending, searchTerm, currentPage, pageSize);
            return View(viewModel);
        }
    }
}
