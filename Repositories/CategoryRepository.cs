
using HuynhNgocTien_SE18B01_A01.Models;

namespace HuynhNgocTien_SE18B01_A01.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FunewsManagementContext _context;

        public CategoryRepository(FunewsManagementContext context)
        {
            _context = context;
        }
        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
    }
}
