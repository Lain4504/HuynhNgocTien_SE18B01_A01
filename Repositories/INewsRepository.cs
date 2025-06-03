
using HuynhNgocTien_SE18B01_A01.Models;

namespace HuynhNgocTien_SE18B01_A01.Repositories
{
    public interface INewsRepository
    {
        void Create(NewsArticle model);
        void Update(NewsArticle model);
        void Delete(NewsArticle model);
        NewsArticle? FindById(string id);

        List<NewsArticle> GetNewsByAccountID(short accountId);

        List<NewsArticle> StatisticNews(DateTime startDate, DateTime endDate);
    }
}
