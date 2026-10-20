namespace Midterm.Models;

public class QuizSettings
{
    public TriviaCategory Category { get; set; } = TriviaCategory.All[0];
    public Difficulty Difficulty { get; set; } = Difficulty.Easy;
    public QuestionType Type { get; set; } = QuestionType.Multiple;
    public int Amount { get; set; } = 10;

    public QuizSettings Clone() => (QuizSettings)MemberwiseClone();
}
