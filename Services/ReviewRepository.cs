using Microsoft.EntityFrameworkCore;
using RabotaGovnoClone.Data;
using RabotaGovnoClone.Models;

namespace RabotaGovnoClone.Services;

public sealed class ReviewRepository
{
    private readonly AppDbContext _db;

    public ReviewRepository(AppDbContext db)
    {
        _db = db;
        SeedIfEmpty();
    }

    public int TotalCount => _db.Reviews.Count();

    public IReadOnlyList<Review> GetLatest(int take = 20)
        => _db.Reviews
            .AsNoTracking()
            .AsEnumerable() // сортировка по DateTimeOffset на клиенте, чтобы обойти ограничение SQLite
            .OrderByDescending(r => r.CreatedAt)
            .Take(take)
            .ToList();

    public IReadOnlyList<Review> GetBest(int take = 20)
        => _db.Reviews
            .AsNoTracking()
            .AsEnumerable() // сортировка по DateTimeOffset на клиенте
            .OrderByDescending(r => r.Score)
            .ThenByDescending(r => r.CreatedAt)
            .Take(take)
            .ToList();

    public Review? GetById(int id)
        => _db.Reviews
            .Include(r => r.Comments)
            .AsNoTracking()
            .FirstOrDefault(r => r.Id == id);

    public Review AddReview(Review review)
    {
        review.CreatedAt = review.CreatedAt == default ? DateTimeOffset.Now : review.CreatedAt;
        _db.Reviews.Add(review);
        _db.SaveChanges();
        return review;
    }

    public Comment? AddComment(int reviewId, string author, string text)
    {
        var review = _db.Reviews
            .Include(r => r.Comments)
            .FirstOrDefault(r => r.Id == reviewId);

        if (review is null) return null;

        var c = new Comment
        {
            Author = string.IsNullOrWhiteSpace(author) ? "Аноним" : author.Trim(),
            Text = text.Trim(),
            CreatedAt = DateTimeOffset.Now,
        };

        review.Comments.Add(c);
        _db.SaveChanges();

        return c;
    }

    private void SeedIfEmpty()
    {
        if (_db.Reviews.Any())
        {
            return;
        }

        // Один отзыв, похожий по структуре на страницу-референс.
        var r = new Review
        {
            Company = "Работа гавно",
            Author = "Елена",
            Category = "Дизайн, рекламные материалы",
            City = "Москва",
            CreatedAt = new DateTimeOffset(2017, 12, 1, 0, 16, 57, TimeSpan.FromHours(3)),
            Score = -13,
            Text = "Работаю в этой компании сравнительно недавно, работаю с клиентами. На данный момент меня все устраивает. Нравится то, что большую часть времени провожу не в офисе, кроме этого, мне нравится общаться и заводить знакомства с новыми людьми. Что касается зарплаты, то вполне достойная, выплачивается без задержек.",
        };
        r.Comments.Add(new Comment
        {
            Author = "Евгения",
            CreatedAt = new DateTimeOffset(2019, 6, 26, 20, 57, 45, TimeSpan.FromHours(3)),
            Text = "Хочу сказать слова благодарности ПЭК за возможность работать в этой компании. Слаженная работа все продумано до мелочей. Я горжусь что работаю в ПЭК.",
        });

        _db.Reviews.Add(r);

        // + немного фейковых для списка
        var rnd = new Random(7);
        var categories = new[] { "IT", "Продажи", "Логистика", "Дизайн", "Маркетинг" };
        var cities = new[] { "Москва", "Санкт‑Петербург", "Екатеринбург", "Казань", "Новосибирск" };
        var companies = new[] { "ООО Ромашка", "ПЭК", "Зебра Софт", "МегаМаркет", "СуперЛогистик" };

        for (var i = 0; i < 25; i++)
        {
            _db.Reviews.Add(new Review
            {
                Company = companies[rnd.Next(companies.Length)],
                Author = rnd.NextDouble() < 0.25 ? "Аноним" : $"Автор{i + 1}",
                Category = categories[rnd.Next(categories.Length)],
                City = cities[rnd.Next(cities.Length)],
                CreatedAt = DateTimeOffset.Now.AddDays(-rnd.Next(1, 1200)).AddMinutes(-rnd.Next(0, 600)),
                Score = rnd.Next(-50, 120),
                Text = "Короткий текст отзыва для демонстрации списка. Замените на реальные данные/БД.",
            });
        }

        _db.SaveChanges();
    }
}
