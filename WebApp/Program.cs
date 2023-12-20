using Application;
using Application.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Refit;
using WebApp.Clients;
using WebApp.Components;
using WebApp.Endpoints;
using WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureApplication();

// Razor Components
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

// Scraper-client
builder.Services.AddRefitClient<IScraperClient>()
    .ConfigureHttpClient(client => 
    {
        client.BaseAddress = new Uri("http://localhost:5227");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    
    app.UseSwaggerUI();
    app.UseSwagger();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapScraperEndpoints();

app.MapHub<ScraperHub>("/scraperhub");

app.Run();