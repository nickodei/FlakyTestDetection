using Application.Features.Github.Models;
using Application.LogParsers;

namespace Application.Features.Github;

public interface IGithubService
{
    Task StartGithubScraping(ScrapingRequest request, ILogFileParser parser, Func<string, Task> logging, CancellationToken cancellationToken);
}