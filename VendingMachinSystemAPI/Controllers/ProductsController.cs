using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendingMachinSystem.Interface;
using VendingMachinSystem.Models;

namespace VendingMachinSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index(string sortColumn = "ProductName", bool isSortDescending = true, string searchTerm = "", int currentPage = 1, int pageSize = 5)
        {
            var viewModel = await _productService.GetProductsAsync(sortColumn, isSortDescending, searchTerm, currentPage, pageSize);
            return Ok(viewModel);
        }

        // POST: products/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Invalid Product");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _productService.CreateProductAsync(product);

            if (success)
                return Ok(new { Id = product.Id, Message = "Product Saved Successfully." });
            else
                return BadRequest(product);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [FromBody] Product product)
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
                    return Ok(new { Id = product.Id, Message = "Product Updated Successfully." });
                }
            }
            return BadRequest(product);
        }

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var success = await _productService.DeleteProductAsync(id);
            if (success)
            {
                return Ok(new { Id = product.Id, Message = "Product Deleted Successfully." });
            }
            return NotFound();
        }

        [HttpPost("purchase/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Purchase(int id, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); ;
            if (userId == null)
            {
                return BadRequest("Invalid Request");
            }

            var success = await _productService.CompletePurchaseAsync(id, quantity, userId);
            if (success)
            {
                return Ok(new { Id = id, Message = "Product purchase Successfully." });
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return BadRequest(product);
        }

        [HttpGet("purchaseList")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PurchaseList(string sortColumn = "ProductName", bool isSortDescending = true, string searchTerm = "", int currentPage = 1, int pageSize = 5)
        {
            var viewModel = await _productService.GetPurchaseListAsync(sortColumn, isSortDescending, searchTerm, currentPage, pageSize);
            return Ok(viewModel);
        }
    }
}
