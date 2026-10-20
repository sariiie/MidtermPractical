using System.Text.Json.Serialization;

namespace Midterm.Models;

public class UserStats
{
    public int GamesPlayed { get; set; }
    public int TotalScore { get; set; }
    public int BestScore { get; set; }
    public int CorrectAnswers { get; set; }
    public int QuestionsAnswered { get; set; }
    public int BestStreak { get; set; }

    /// <summary>"S", "A", "B", "C" or "D". Empty until the first game is finished.</summary>
    public string BestRank { get; set; } = "";

    [JsonIgnore]
    public double Accuracy => QuestionsAnswered == 0 ? 0 : (double)CorrectAnswers / QuestionsAnswered;
}
