using Microsoft.EntityFrameworkCore;
using RateLimiter.Api.Application;
using RateLimiter.Api.Grpc;
using RateLimiter.Api.Services;
using RateLimiter.Persistence.Db;
using RateLimiter.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<RateLimiterDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddScoped<IRateLimitPolicyRepository, RateLimitPolicyRepository>();
builder.Services.AddScoped<ITokenBucketStateRepository, TokenBucketStateRepository>();

builder.Services.AddScoped<IRateLimiterApplicationService, RateLimiterApplicationService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<RateLimiterGrpcService>();
app.MapHealthChecks("/health");

app.Run();
