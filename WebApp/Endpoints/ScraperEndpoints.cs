using Application.Features.Github;
using Application.Features.Github.Models;
using Application.LogParsers.Parsers;
using Microsoft.AspNetCore.SignalR;
using WebApp.Components;
using WebApp.Hubs;

namespace WebApp.Endpoints;

public static class ScraperEndpoints
{
    public static CancellationTokenSource? CancellationTokenSource;
    public static IEndpointRouteBuilder MapScraperEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/scraper/start", (ScrapingRequest request, IGithubService githubService, IHubContext<ScraperHub> hubContext) =>
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = CancellationTokenSource.Token;
            
            Task.Run(() =>
            {
                githubService.StartGithubScraping(request, new PlaywrightLogParser(), (msg) =>
                {
                    return hubContext.Clients.All.SendAsync("ReceiveLog", msg);
                }, cancellationToken);
            }, CancellationTokenSource.Token);
        });
        
        endpoints.MapPost("/scraper/stop", () =>
        {
            CancellationTokenSource?.Cancel();
        });

        return endpoints;
    }
}