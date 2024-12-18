using Azure;
using Azure.AI.Inference;
using Azure.AI.OpenAI;
using DotNetEnv;
using lightsmeai.Controllers;
using Microsoft.Extensions.AI;

// Get keys from configuration
Env.Load(".env");
string githubKey = Env.GetString("GITHUB_KEY");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the chat client
IChatClient innerChatClient = new ChatCompletionsClient(
    endpoint: new Uri("https://models.inference.ai.azure.com"),
    new AzureKeyCredential(githubKey))
    .AsChatClient("gpt-4o-mini");

builder.Services.AddChatClient(chatClientBuilder => chatClientBuilder
    .UseFunctionInvocation()
    .UseLogging()
    .Use(innerChatClient));

// Register embedding generator
builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
    new AzureOpenAIClient(new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubKey))
        .AsEmbeddingGenerator(modelId: "text-embedding-3-large"));

builder.Services.AddLogging(loggingBuilder =>
    loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Trace));

var light = new LightController();
var getAllLightsTool = AIFunctionFactory.Create(light.GetLights);
var getLightTool = AIFunctionFactory.Create(light.GetLight);
var createLightTool = AIFunctionFactory.Create(light.AddLight);
var updateLightTool = AIFunctionFactory.Create(light.UpdateLight);
var deleteLightTool = AIFunctionFactory.Create(light.DeleteLight);

var chatOptions = new ChatOptions
{
    Tools = new[]
    {
        getAllLightsTool,
        getLightTool,
        createLightTool,
        updateLightTool,
        deleteLightTool
        }
};

builder.Services.AddSingleton(light);
builder.Services.AddSingleton(chatOptions);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // Enable serving static files
app.UseRouting(); // Must come before UseEndpoints
app.UseAuthorization();
app.MapControllers();
// Serve index.html as the default page 
app.MapFallbackToFile("index.html");
app.Run();