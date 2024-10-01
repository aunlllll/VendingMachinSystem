using VendingMachinSystem.Models;

namespace VendingMachinSystem.ViewModels
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public bool IsSortDescending { get; set; }
        public string SearchTerm { get; set; }
    }
}
