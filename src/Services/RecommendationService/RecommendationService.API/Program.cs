using Microsoft.SemanticKernel;
using RecommendationService.Application.Queries.GetRecommendations;
using RecommendationService.Application.Services;
using RecommendationService.Infrastructure.AI;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Semantic Kernel
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"] ?? "placeholder-key-for-build";
var modelId = builder.Configuration["OpenAI:ModelId"] ?? "gpt-4o-mini";

builder.Services.AddKernel()
    .AddOpenAIChatCompletion(modelId, openAiApiKey);

// Infrastructure
builder.Services.AddScoped<IAIRecommendationService, OpenAIRecommendationService>();

// Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetRecommendationsQuery>());

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();