# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY RateLimiter.sln .
COPY src ./src

RUN dotnet restore
RUN dotnet publish src/RateLimiter.Api \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ---------- runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RateLimiter.Api.dll"]
