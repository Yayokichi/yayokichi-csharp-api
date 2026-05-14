var builder = WebApplication.CreateBuilder(args);

// CORSの設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// --- 既存の /api/status エンドポイント ---
app.MapGet("/api/status", () =>
{
    return Results.Json(new
    {
        Message = "Hello from C# Microservice!",
        Status = "Active",
        Language = "C# (.NET 8)",
        Timestamp = DateTime.UtcNow.ToString("o") // ISO 8601形式
    });
});


// ★★★ ここからがLINQデモ用の新しいコード ★★★

// 1. 商品データを表現するレコード型を定義
public record Product(int Id, string Name, string Category, decimal Price);

// 2. ダミーの商品データを作成して保持する静的クラス
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
        // ポートフォリオの見栄えを良くするため、実際には20〜30件程度あるとよりリッチに見えます
    };

    public static List<Product> GetAll() => _products;
}

// 3. LINQを使ってフィルタリングとソートを行う新しいAPIエンドポイント
app.MapGet("/api/products", (string? category, string? sortBy, string? sortOrder) =>
{
    // IQueryableを使うことで、複数のLINQメソッドを効率的に連結できる
    var query = ProductService.GetAll().AsQueryable();

    // --- フィルタリング (LINQ: Where) ---
    if (!string.IsNullOrEmpty(category) && category != "All")
    {
        query = query.Where(p => p.Category == category);
    }

    // --- 並び替え (LINQ: OrderBy / OrderByDescending) ---
    // sortByパラメータに応じて並び替えのキーを動的に変更
    // sortOrderパラメータに応じて昇順・降順を切り替え
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
            // デフォルトはID順
            query = query.OrderBy(p => p.Id);
            break;
    }

    var results = query.ToList();
    return Results.Ok(results);
});


// --- 起動設定 ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");