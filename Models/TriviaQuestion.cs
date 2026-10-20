namespace Midterm.Models;

public class TriviaQuestion
{
    public string Text { get; set; } = "";
    public string CorrectAnswer { get; set; } = "";

    /// <summary>All answers, already shuffled (True/False is always in that order).</summary>
    public List<string> Answers { get; set; } = new();

    public string Category { get; set; } = "";
    public Difficulty Difficulty { get; set; }
    public QuestionType Type { get; set; }
}
