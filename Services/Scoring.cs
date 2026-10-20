using Midterm.Models;

namespace Midterm.Services;

public static class Scoring
{
    /// <param name="fractionRemaining">1.0 = just started, 0.0 = out of time</param>
    public static double Multiplier(double fractionRemaining) => fractionRemaining switch
    {
        >= 0.75 => 2.0,
        >= 0.5 => 1.5,
        >= 0.25 => 1.25,
        _ => 1.0
    };

    public static int Points(Difficulty difficulty, double fractionRemaining) =>
        (int)Math.Round(difficulty.BasePoints() * Multiplier(fractionRemaining));
}
