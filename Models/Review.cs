namespace RabotaGovnoClone.Models;

public sealed class Review
{
    public int Id { get; set; }
    public string Company { get; set; } = "";
    public string Author { get; set; } = "";
    public string Category { get; set; } = "";
    public string City { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public int Score { get; set; } // simple +/- rating
    public string Text { get; set; } = "";

    public List<Comment> Comments { get; set; } = new();
}
