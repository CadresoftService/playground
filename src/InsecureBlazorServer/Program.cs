\
/*
  Intentionally insecure demo app for GHAS/CodeQL training.
  Do NOT deploy. Do NOT expose to the internet.
*/
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Hardcoded secret (CodeQL often flags hardcoded credentials/keys)
const string HardcodedApiKey = "sk_test_51ExampleHardcodedSecret_DoNotUse";

// -------------------------
// Intentionally insecure API endpoints (training)
// -------------------------

// Reflected XSS: returns user input in HTML without encoding
app.MapGet("/api/xss", (string q) =>
{
    return Results.Content($"<h1>You searched for: {q}</h1>", "text/html");
});

// Open redirect: redirect target controlled by user input
app.MapGet("/api/redirect", (string url) => Results.Redirect(url));

// Path traversal-like: reads a file path based on user input
app.MapGet("/api/file", ([FromQuery] string path) =>
{
    var content = System.IO.File.ReadAllText(path);
    return Results.Text(content);
});

// SSRF-like: fetches arbitrary URL provided by user
app.MapGet("/api/fetch", async ([FromQuery] string url) =>
{
    using var http = new HttpClient();
    var text = await http.GetStringAsync(url);
    return Results.Text(text);
});

// Risky JSON deserialization configuration
app.MapPost("/api/deserialize", async (HttpRequest req) =>
{
    using var sr = new StreamReader(req.Body);
    var json = await sr.ReadToEndAsync();

    var settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto // risky in many contexts
    };

    var obj = JsonConvert.DeserializeObject<object>(json, settings);
    return Results.Json(new { ok = true, type = obj?.GetType().FullName });
});

// Weak crypto: MD5 hashing used for password-like input
app.MapPost("/api/hash", ([FromBody] HashRequest body) =>
{
    using var md5 = MD5.Create();
    var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(body.Input ?? ""));
    return Results.Json(new { md5 = Convert.ToHexString(bytes) });
});

// Insecure randomness: System.Random used for tokens
app.MapGet("/api/token", () =>
{
    var rnd = new Random();
    var token = rnd.Next(0, int.MaxValue).ToString();
    return Results.Json(new { token });
});

// Missing auth on sensitive endpoint: returns hardcoded API key
app.MapGet("/api/config", () => Results.Json(new { apiKey = HardcodedApiKey }));

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public record HashRequest(string? Input);
