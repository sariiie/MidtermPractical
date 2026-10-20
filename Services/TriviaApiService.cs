using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Midterm.Models;

namespace Midterm.Services;

public class TriviaApiException(string message) : Exception(message);

/// <summary>Talks to the Open Trivia DB API and maps results to our models.</summary>
public class TriviaApiService(HttpClient http)
{
    private const string RateLimitMessage =
        "Slow down! Open Trivia DB allows one request every 5 seconds. Wait a moment and try again.";

    public async Task<List<TriviaQuestion>> GetQuestionsAsync(QuizSettings s, CancellationToken ct = default)
    {
        var url = $"api.php?amount={s.Amount}&category={s.Category.Id}" +
                  $"&difficulty={s.Difficulty.ToApi()}&type={s.Type.ToApi()}";

        OpenTdbResponse? data;
        try
        {
            data = await http.GetFromJsonAsync<OpenTdbResponse>(url, ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new TriviaApiException(RateLimitMessage);
        }
        catch (HttpRequestException)
        {
            throw new TriviaApiException("Couldn't reach Open Trivia DB. Check your internet connection and try again.");
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TriviaApiException("Open Trivia DB took too long to answer. Try again.");
        }
        catch (JsonException)
        {
            throw new TriviaApiException("Open Trivia DB sent back something unreadable. Try again.");
        }

        if (data is null)
            throw new TriviaApiException("Open Trivia DB sent back an empty response. Try again.");

        switch (data.ResponseCode)
        {
            case 0:
                break;
            case 1:
                throw new TriviaApiException(
                    $"Open Trivia DB doesn't have {s.Amount} {s.Difficulty} {s.Type.Label()} questions for {s.Category.Name}. " +
                    "Try fewer questions, a different difficulty, or another format.");
            case 5:
                throw new TriviaApiException(RateLimitMessage);
            default:
                throw new TriviaApiException($"Open Trivia DB returned an unexpected error (code {data.ResponseCode}).");
        }

        return data.Results.Select(q => Map(q, s)).ToList();
    }

    private static TriviaQuestion Map(OpenTdbQuestion q, QuizSettings s)
    {
        // The API HTML-encodes text (&quot; &#039; ...), so decode it.
        var correct = Decode(q.CorrectAnswer);
        string[] answers = q.IncorrectAnswers.Select(Decode).Append(correct).ToArray();

        if (s.Type == QuestionType.Boolean)
            answers = new[] { "True", "False" };
        else
            Random.Shared.Shuffle(answers);

        return new TriviaQuestion
        {
            Text = Decode(q.Question),
            CorrectAnswer = correct,
            Answers = answers.ToList(),
            Category = s.Category.Name,
            Difficulty = s.Difficulty,
            Type = s.Type
        };
    }

    private static string Decode(string value) => WebUtility.HtmlDecode(value) ?? "";
}
