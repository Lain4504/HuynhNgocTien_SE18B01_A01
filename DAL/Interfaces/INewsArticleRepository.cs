using DAL.Entities;

namespace DAL.Interfaces;

public interface INewsArticleRepository
{
    Task<NewsArticle?> GetByIdAsync(string id);
    Task<IEnumerable<NewsArticle>> GetAllAsync();
    Task<IEnumerable<NewsArticle>> GetByCategoryAsync(short categoryId);
    Task<IEnumerable<NewsArticle>> GetByAuthorAsync(short authorId);
    Task<IEnumerable<NewsArticle>> GetByTagAsync(int tagId);
    Task<NewsArticle> AddAsync(NewsArticle article);
    Task<NewsArticle> UpdateAsync(NewsArticle article);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
} 