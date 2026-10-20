namespace Midterm.Models;

public enum Difficulty { Easy, Medium, Hard }

public enum QuestionType { Multiple, Boolean }

public static class QuizOptionExtensions
{
    // Values the Open Trivia DB API expects
    public static string ToApi(this Difficulty d) => d.ToString().ToLowerInvariant();

    public static string ToApi(this QuestionType t) =>
        t == QuestionType.Multiple ? "multiple" : "boolean";

    public static string Label(this QuestionType t) =>
        t == QuestionType.Multiple ? "Multiple choice" : "True / False";

    public static int BasePoints(this Difficulty d) => d switch
    {
        Difficulty.Easy => 100,
        Difficulty.Medium => 200,
        _ => 300
    };

    public static int TimeLimitSeconds(this Difficulty d) => d switch
    {
        Difficulty.Easy => 20,
        Difficulty.Medium => 15,
        _ => 12
    };
}
