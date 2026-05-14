var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// --- エンドポイントの定義 ---

// 1. 既存の /api/status エンドポイント
app.MapGet("/api/status", () =>
{
    return Results.Json(new
    {
        Message = "Hello from C# Microservice!",
        Status = "Active",
        Language = "C# (.NET 8)",
        Timestamp = DateTime.UtcNow.ToString("o")
    });
});

// 2. 新しい /api/products エンドポイント
app.MapGet("/api/products", (string? category, string? sortBy, string? sortOrder) =>
{
    var query = ProductService.GetAll().AsQueryable();

    if (!string.IsNullOrEmpty(category) && category != "All")
    {
        query = query.Where(p => p.Category == category);
    }

    switch (sortBy?.ToLower())
    {
        case "price":
            query = sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price);
            break;
        case "name":
            query = sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name);
            break;
        default:
            query = query.OrderBy(p => p.Id);
            break;
    }

    var results = query.ToList();
    return Results.Ok(results);
});

// --- 起動設定 ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");


// ★ 修正点: 型定義とデータクラスをファイルの末尾に移動 ★
// これにより、トップレベルステートメントのスコープ問題を確実に回避します。

public record Product(int Id, string Name, string Category, decimal Price);

public static class ProductService
{
    private static readonly List<Product> _products = new()
    {
        new(1, "入門Go言語", "書籍", 3200),
        new(2, "実践ドメイン駆動設計", "書籍", 5800),
        new(3, "ワイヤレスイヤホン Pro", "家電", 28000),
        new(4, "4K液晶モニター 27インチ", "家電", 45000),
        new(5, "電動コーヒーミル", "キッチン用品", 8500),
        new(6, "高級オフィスチェア", "家具", 76000),
        new(7, "C#クックブック", "書籍", 4200),
        new(8, "スマートスピーカー Mini", "家電", 5980),
        new(9, "ステンレス製タンブラー", "キッチン用品", 2500),
        new(10, "人間工学キーボード", "家電", 18000),
        new(11, "速習PHP", "書籍", 2900),
        new(12, "ノイズキャンセリングヘッドホン", "家電", 35000)
    };

    public static List<Product> GetAll() => _products;
}