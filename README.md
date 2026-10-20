# TriviaMaster Arcade

Blazor Web App (Interactive Server) + C# + Tailwind CSS v4. Questions come live from
[Open Trivia DB](https://opentdb.com/api_config.php), so there are no images or data files to manage.

## Run it

Terminal 1 (Tailwind):

    npm install
    npm run tw:watch

Terminal 2 (app):

    dotnet watch

Then open http://localhost:5080

`TargetFramework` in `Midterm.csproj` is `net8.0`. Change it to match your installed SDK
(`dotnet --version`), e.g. `net9.0` or `net10.0`.

## Accounts (front end only)

Register / log in / guest play are handled by `Services/AuthService.cs`. There is no database:
accounts and stats are saved in the browser's localStorage (passwords are salted + PBKDF2-hashed).
They exist only in the browser where you created them. This is a demo, not real security.

## Layout

- `Models/`: settings, questions, answer records, results, and the Open Trivia DB JSON shapes
- `Services/TriviaApiService.cs`: fetches and decodes questions
- `Services/Scoring.cs`: speed multipliers (x2 / x1.5 / x1.25 / x1)
- `Services/AuthService.cs`: browser-only accounts, session, and per-user stats
- `Services/GameSession.cs`: game state shared between pages
- `Components/Pages/`: Home (setup + player card), Play (quiz), Results (breakdown + confetti), Login, Register
- `wwwroot/js/trivia.js`: confetti
