using System.Data;
using Grpc.Core;
using RateLimiter.Api.Application;
using RateLimiter.Grpc;


namespace RateLimiter.Api.Grpc;

public sealed class RateLimiterGrpcService : RateLimiter.Grpc.RateLimiter.RateLimiterBase
{
    private readonly IRateLimiterApplicationService _application;

    public RateLimiterGrpcService(IRateLimiterApplicationService application)
    {
        _application = application;
    }
    
    public override async Task<RateLimitResponse> Check(RateLimitRequest request, ServerCallContext context)
    {
        if (request.Cost <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Cost must be greater than 0"));
        }

        var decision = await _application.CheckAsync(request.Subject, request.Resource, request.Cost, context.CancellationToken);

        return new RateLimitResponse
        {
            Allowed = decision.Allowed,
            PolicyId = decision.PolicyId,
            RemainingTokens = decision.RemainingTokens,
            RetryAfterSeconds = decision.RetryAfter?.Seconds ?? 0
        };
    }
}