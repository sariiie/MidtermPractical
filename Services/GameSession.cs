using Midterm.Models;

namespace Midterm.Services;

/// <summary>Holds the current game between pages. Registered as Scoped (one per browser tab).</summary>
public class GameSession
{
    public QuizSettings Settings { get; private set; } = new();
    public List<TriviaQuestion> Questions { get; private set; } = new();
    public List<AnswerRecord> Answers { get; } = new();
    public QuizResult? Result { get; private set; }

    /// <summary>Set after a game ends: did it beat the player's previous best score?</summary>
    public bool IsNewBest { get; set; }

    public bool HasActiveGame => Questions.Count > 0 && Result is null;

    public void Start(QuizSettings settings, IEnumerable<TriviaQuestion> questions)
    {
        Settings = settings;
        Questions = questions.ToList();
        Answers.Clear();
        Result = null;
        IsNewBest = false;
    }

    public void Finish() => Result = QuizResult.From(Settings, Answers);

    public void Reset()
    {
        Questions = new();
        Answers.Clear();
        Result = null;
        IsNewBest = false;
    }
}
