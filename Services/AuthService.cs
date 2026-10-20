using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.JSInterop;
using Midterm.Models;

namespace Midterm.Services;

/// <summary>
/// Front-end-only accounts. Everything lives in the browser's localStorage, so there is no database.
/// This is a demo: accounts exist only on this browser and are not real security.
/// </summary>
public class AuthService(IJSRuntime js)
{
    private const string UsersKey = "tm.users";
    private const string SessionKey = "tm.session";
    private const string GuestId = "guest";
    private const int Iterations = 100_000;

    private static string StatsKey(string id) => $"tm.stats.{id}";

    private bool _initialized;

    public UserAccount? CurrentUser { get; private set; }
    public UserStats Stats { get; private set; } = new();
    public bool IsInitialized { get; private set; }
    public bool IsAuthenticated => CurrentUser is not null;
    public bool IsGuest => CurrentUser?.IsGuest == true;

    public event Action? OnChange;

    /// <summary>Restores the signed-in user from localStorage. Safe to call more than once.</summary>
    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;

        var id = await GetAsync(SessionKey);
        if (id == GuestId)
        {
            CurrentUser = NewGuest();
        }
        else if (!string.IsNullOrEmpty(id))
        {
            var users = await LoadUsersAsync();
            CurrentUser = users.FirstOrDefault(u => u.Id == id);
        }

        await LoadStatsAsync();
        IsInitialized = true;
        OnChange?.Invoke();
    }

    public async Task<AuthResult> RegisterAsync(RegisterForm form)
    {
        var users = await LoadUsersAsync();
        var email = form.Email.Trim().ToLowerInvariant();

        if (users.Any(u => u.Email == email))
            return AuthResult.Fail("An account with that email already exists. Try logging in instead.");

        var salt = RandomNumberGenerator.GetBytes(16);
        var user = new UserAccount
        {
            Id = Guid.NewGuid().ToString("N"),
            DisplayName = form.DisplayName.Trim(),
            Email = email,
            Avatar = form.Avatar,
            Salt = Convert.ToBase64String(salt),
            PasswordHash = Hash(form.Password, salt)
        };

        users.Add(user);
        await SetAsync(UsersKey, JsonSerializer.Serialize(users));
        await SignInAsync(user);
        return AuthResult.Ok();
    }

    public async Task<AuthResult> LoginAsync(LoginForm form)
    {
        var users = await LoadUsersAsync();
        var email = form.Email.Trim().ToLowerInvariant();
        var user = users.FirstOrDefault(u => u.Email == email);

        // Same message for "no such email" and "wrong password"
        if (user is null || !Verify(form.Password, user))
            return AuthResult.Fail("Email or password is incorrect.");

        await SignInAsync(user);
        return AuthResult.Ok();
    }

    public async Task ContinueAsGuestAsync()
    {
        CurrentUser = NewGuest();
        Stats = new UserStats();
        await SetAsync(SessionKey, GuestId);
        OnChange?.Invoke();
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        Stats = new UserStats();
        await RemoveAsync(SessionKey);
        OnChange?.Invoke();
    }

    /// <summary>Adds a finished game to the player's stats. Returns true if it beat their previous best score.</summary>
    public async Task<bool> RecordGameAsync(QuizResult result)
    {
        var hadGames = Stats.GamesPlayed > 0;
        var previousBest = Stats.BestScore;

        Stats.GamesPlayed++;
        Stats.TotalScore += result.TotalScore;
        Stats.CorrectAnswers += result.Correct;
        Stats.QuestionsAnswered += result.Total;
        Stats.BestStreak = Math.Max(Stats.BestStreak, result.BestStreak);
        Stats.BestScore = Math.Max(Stats.BestScore, result.TotalScore);
        if (RankOrder(result.Rank) < RankOrder(Stats.BestRank))
            Stats.BestRank = result.Rank;

        // Guests keep stats for this visit only
        if (CurrentUser is { IsGuest: false } user)
            await SetAsync(StatsKey(user.Id), JsonSerializer.Serialize(Stats));

        OnChange?.Invoke();
        return hadGames && result.TotalScore > previousBest;
    }

    // ---- internals ----

    private async Task SignInAsync(UserAccount user)
    {
        CurrentUser = user;
        await SetAsync(SessionKey, user.Id);
        await LoadStatsAsync();
        OnChange?.Invoke();
    }

    private async Task LoadStatsAsync()
    {
        Stats = new UserStats();
        if (CurrentUser is not { IsGuest: false } user) return;

        var json = await GetAsync(StatsKey(user.Id));
        if (string.IsNullOrEmpty(json)) return;

        try { Stats = JsonSerializer.Deserialize<UserStats>(json) ?? new UserStats(); }
        catch (JsonException) { /* corrupt data: start fresh */ }
    }

    private async Task<List<UserAccount>> LoadUsersAsync()
    {
        var json = await GetAsync(UsersKey);
        if (string.IsNullOrEmpty(json)) return new List<UserAccount>();

        try { return JsonSerializer.Deserialize<List<UserAccount>>(json) ?? new List<UserAccount>(); }
        catch (JsonException) { return new List<UserAccount>(); }
    }

    private static UserAccount NewGuest() => new()
    {
        Id = GuestId,
        DisplayName = "Guest",
        Avatar = "👤",
        IsGuest = true
    };

    private static int RankOrder(string rank) =>
        string.IsNullOrEmpty(rank) ? int.MaxValue : "SABCD".IndexOf(rank, StringComparison.Ordinal);

    private static string Hash(string password, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32));

    private static bool Verify(string password, UserAccount user)
    {
        try
        {
            var salt = Convert.FromBase64String(user.Salt);
            var expected = Convert.FromBase64String(user.PasswordHash);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    // localStorage helpers (swallow errors so private-mode browsers don't crash the app)
    private async Task<string?> GetAsync(string key)
    {
        try { return await js.InvokeAsync<string?>("trivia.store.get", key); }
        catch (Exception) { return null; }
    }

    private async Task SetAsync(string key, string value)
    {
        try { await js.InvokeVoidAsync("trivia.store.set", key, value); }
        catch (Exception) { }
    }

    private async Task RemoveAsync(string key)
    {
        try { await js.InvokeVoidAsync("trivia.store.remove", key); }
        catch (Exception) { }
    }
}
