using Application.LogParsers.Parsers;
using Microsoft.AspNetCore.SignalR;

namespace WebApp.Hubs;

public class ScraperHub() : Hub
{
    public Task StopGithubScraping()
    {
        return Task.CompletedTask;
    }
}