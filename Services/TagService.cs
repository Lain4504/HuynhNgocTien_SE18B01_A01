using HuynhNgocTien_SE18B01_A01.Models;
using HuynhNgocTien_SE18B01_A01.Repositories;

namespace HuynhNgocTien_SE18B01_A01.Services
{
    public class TagService
    {
        private readonly ITagRepository _repo;
        public TagService(ITagRepository repo)
        {
            _repo = repo;
        }

        public List<Tag> GetListTag()
        {
            return _repo.GetListTag();
        }
    }
}
