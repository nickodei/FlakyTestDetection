using Application.Features.Github.Models;
using Refit;

namespace WebApp.Clients;

public interface IScraperClient
{
    [Post("/scraper/start")]
    Task StartScraping(ScrapingRequest request);
    
    [Post("/scraper/stop")]
    Task StopScraping();
}