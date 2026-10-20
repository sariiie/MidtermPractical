using System.Text.Json.Serialization;

namespace Midterm.Models;

// Shapes of the JSON returned by https://opentdb.com/api.php

public class OpenTdbResponse
{
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }

    [JsonPropertyName("results")]
    public List<OpenTdbQuestion> Results { get; set; } = new();
}

public class OpenTdbQuestion
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("question")]
    public string Question { get; set; } = "";

    [JsonPropertyName("correct_answer")]
    public string CorrectAnswer { get; set; } = "";

    [JsonPropertyName("incorrect_answers")]
    public List<string> IncorrectAnswers { get; set; } = new();
}
