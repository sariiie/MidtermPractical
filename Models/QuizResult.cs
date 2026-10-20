namespace Midterm.Models;

public class QuizResult
{
    public QuizSettings Settings { get; init; } = new();
    public IReadOnlyList<AnswerRecord> Answers { get; init; } = Array.Empty<AnswerRecord>();

    public int TotalScore => Answers.Sum(a => a.Points);
    public int Total => Answers.Count;
    public int Correct => Answers.Count(a => a.IsCorrect);
    public double Accuracy => Total == 0 ? 0 : (double)Correct / Total;
    public double AverageSeconds => Total == 0 ? 0 : Answers.Average(a => a.TimeTaken.TotalSeconds);

    public int BestStreak
    {
        get
        {
            int best = 0, current = 0;
            foreach (var a in Answers)
            {
                current = a.IsCorrect ? current + 1 : 0;
                best = Math.Max(best, current);
            }
            return best;
        }
    }

    public string Rank => Accuracy switch
    {
        >= 0.9 => "S",
        >= 0.75 => "A",
        >= 0.6 => "B",
        >= 0.4 => "C",
        _ => "D"
    };

    public string RankTitle => Accuracy switch
    {
        >= 0.9 => "Trivia legend",
        >= 0.75 => "Quiz whiz",
        >= 0.6 => "Solid player",
        >= 0.4 => "Warming up",
        _ => "Rookie. Run it back!"
    };

    public static QuizResult From(QuizSettings settings, IEnumerable<AnswerRecord> answers) => new()
    {
        Settings = settings,
        Answers = answers.ToList()
    };
}
