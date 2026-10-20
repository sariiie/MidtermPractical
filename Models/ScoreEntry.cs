namespace Midterm.Models;

/// <summary>One finished game on the leaderboard.</summary>
public class ScoreEntry
{
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Avatar { get; set; } = "👤";
    public int Score { get; set; }
    public string Rank { get; set; } = "D";
    public string Category { get; set; } = "";
    public string CategoryEmoji { get; set; } = "";
    public Difficulty Difficulty { get; set; }
    public QuestionType Type { get; set; }
    public int Correct { get; set; }
    public int Total { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
}
