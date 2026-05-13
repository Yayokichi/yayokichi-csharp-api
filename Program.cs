var builder = WebApplication.CreateBuilder(args);

// CORSの設定（Next.jsから呼び出せるようにする）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// CORSポリシーを適用
app.UseCors("AllowAll");

// APIエンドポイントの設定
app.MapGet("/api/status", () =>
{
    return Results.Json(new
    {
        Message = "Hello from C# Microservice!",
        Status = "Active",
        Language = "C# (.NET 8)",
        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
    });
});

// Renderの環境変数PORTを取得して起動する設定
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");