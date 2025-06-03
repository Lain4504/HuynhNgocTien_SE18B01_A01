using HuynhNgocTien_SE18B01_A01.Models;
using HuynhNgocTien_SE18B01_A01.Repositories;

namespace HuynhNgocTien_SE18B01_A01.Services
{
    public class NewsService
    {
        private readonly INewsRepository _repo;

        public NewsService(INewsRepository repo)
        {
            _repo = repo;
        }

        public void Create(NewsArticle model)
        {
            _repo.Create(model);
        }
        
        public void Update(NewsArticle model)
        {
            _repo.Update(model);
        }

        public void Delete(NewsArticle model)
        {
            _repo.Delete(model);
        }

        public NewsArticle? FindById(string id)
        {
            return _repo.FindById(id);
        }

        public List<NewsArticle> GetNewsByAccountID(short accountId)
        {
            return _repo.GetNewsByAccountID(accountId);
        }

        public List<NewsArticle> StatisticNews(DateTime startDate, DateTime endDate)
        {
            return _repo.StatisticNews(startDate, endDate);
        }
    }
}
