using Microsoft.EntityFrameworkCore;
using WebAPI;
using WebAPI.Utility;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterModules(builder.Configuration);
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<FlakyTestDetectionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FlakyTestDetection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapEndpoints();
app.Run();