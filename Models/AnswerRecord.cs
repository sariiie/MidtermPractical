namespace Midterm.Models;

public class AnswerRecord
{
    public TriviaQuestion Question { get; init; } = default!;
    public string? SelectedAnswer { get; init; }
    public bool IsCorrect { get; init; }
    public bool TimedOut { get; init; }
    public TimeSpan TimeTaken { get; init; }
    public double Multiplier { get; init; }
    public int Points { get; init; }
}
