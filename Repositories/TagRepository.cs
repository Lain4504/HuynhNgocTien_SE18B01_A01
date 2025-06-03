using HuynhNgocTien_SE18B01_A01.Models;

namespace HuynhNgocTien_SE18B01_A01.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly FunewsManagementContext _context;

        public TagRepository(FunewsManagementContext context) {  _context = context; }
        public List<Tag> GetListTag()
        {
            return _context.Tags.ToList();
        }
    }
}
