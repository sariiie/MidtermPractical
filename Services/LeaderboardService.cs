using System.Text.Json;
using Microsoft.JSInterop;
using Midterm.Models;

namespace Midterm.Services;

/// <summary>
/// Browser-only leaderboard. Every finished game is saved to localStorage (top scores only),
/// so it lists everyone who has played on this browser. Same idea as AuthService: no database.
/// </summary>
public class LeaderboardService(IJSRuntime js)
{
    private const string Key = "tm.scores";
    private const int MaxEntries = 200;

    public async Task<List<ScoreEntry>> GetAllAsync()
    {
        try
        {
            var json = await js.InvokeAsync<string?>("trivia.store.get", Key);
            if (string.IsNullOrEmpty(json)) return new List<ScoreEntry>();
            return JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
        }
        catch (Exception)
        {
            // Private-mode browser or corrupt data: show an empty board instead of crashing
            return new List<ScoreEntry>();
        }
    }

    /// <summary>Saves a finished game. Returns its overall place (1 = best), or 0 if it didn't make the board.</summary>
    public async Task<int> AddAsync(UserAccount? user, QuizResult result)
    {
        if (result.TotalScore <= 0) return 0;

        var all = await GetAllAsync();
        var entry = new ScoreEntry
        {
            UserId = user?.Id ?? "guest",
            Name = user?.DisplayName ?? "Guest",
            Avatar = user?.Avatar ?? "👤",
            Score = result.TotalScore,
            Rank = result.Rank,
            Category = result.Settings.Category.Name,
            CategoryEmoji = result.Settings.Category.Emoji,
            Difficulty = result.Settings.Difficulty,
            Type = result.Settings.Type,
            Correct = result.Correct,
            Total = result.Total,
            PlayedAt = DateTime.UtcNow
        };

        all.Add(entry);
        all = all.OrderByDescending(e => e.Score).ThenBy(e => e.PlayedAt).Take(MaxEntries).ToList();

        try { await js.InvokeVoidAsync("trivia.store.set", Key, JsonSerializer.Serialize(all)); }
        catch (Exception) { }

        var index = all.IndexOf(entry);
        return index < 0 ? 0 : index + 1;
    }

    public async Task ClearAsync()
    {
        try { await js.InvokeVoidAsync("trivia.store.remove", Key); }
        catch (Exception) { }
    }
}
