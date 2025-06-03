
using HuynhNgocTien_SE18B01_A01.Models;
using HuynhNgocTien_SE18B01_A01.Repositories;

namespace HuynhNgocTien_SE18B01_A01.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public List<Category> GetCategories() 
        {
            return _repo.GetCategories();
        }
    }
}
