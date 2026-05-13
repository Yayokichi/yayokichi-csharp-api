# ビルド用の環境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["yayokichi-csharp-api.csproj", "./"]
RUN dotnet restore "yayokichi-csharp-api.csproj"
COPY . .
RUN dotnet publish "yayokichi-csharp-api.csproj" -c Release -o /app/publish

# 実行用の環境（軽量化）
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "yayokichi-csharp-api.dll"]