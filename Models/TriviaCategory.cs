namespace Midterm.Models;

/// <summary>A category offered in the arcade, mapped to an Open Trivia DB category id.</summary>
public record TriviaCategory(int Id, string Name, string Emoji)
{
    public static readonly IReadOnlyList<TriviaCategory> All = new[]
    {
        new TriviaCategory(11, "Movies", "🎬"),
        new TriviaCategory(15, "Video Games", "🎮"),
        new TriviaCategory(12, "Music", "🎵"),
        new TriviaCategory(31, "Anime & Manga", "🌸"),
        new TriviaCategory(14, "Television", "📺"),
    };
}
