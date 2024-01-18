using System.Net.Http.Headers;
using Application.Common;
using Application.Features.Github;
using Application.Features.Github.Clients;
using Application.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.AddTransient<RetryHandler>();
        services.AddRefitClient<IGithubApiService>(new RefitSettings 
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                })
            })
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://api.github.com/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("flaky-test-detection");
                
            }).AddHttpMessageHandler<RetryHandler>();
        
        services.AddHttpClient("GithubLogClient", client =>
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("flaky-test-detection");
        }).AddHttpMessageHandler<RetryHandler>();
        
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseNpgsql("User ID=postgres;Password=password;Server=postgres;Port=5432;Database=FlakyTestDetection;Pooling=true;")
        );
        
        services.AddSingleton<IGithubService, GithubService>();
        
        return services;
    }
}