using Midterm.Components;
using Midterm.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Open Trivia DB client (https://opentdb.com/api_config.php)
builder.Services.AddHttpClient<TriviaApiService>(client =>
{
    client.BaseAddress = new Uri("https://opentdb.com/");
    client.Timeout = TimeSpan.FromSeconds(12);
});

// One GameSession per browser tab (Blazor circuit)
builder.Services.AddScoped<GameSession>();

// Browser-only accounts (localStorage), no database
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
