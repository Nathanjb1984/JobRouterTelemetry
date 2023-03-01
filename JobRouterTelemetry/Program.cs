using Azure.Communication.JobRouter;
using Azure.Messaging.EventGrid;
using JasonShave.Azure.Communication.Service.EventHandler;
using JasonShave.Azure.Communication.Service.EventHandler.JobRouter;
using JobRouterTelemetry;
using JobRouterTelemetry.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<EventService>();

builder.Services.AddCosmosPersistence(options => builder.Configuration.Bind("Db", options));

builder.Services.AddEventHandlerServices() //<--adds common event handling services
    .AddJobRouterEventHandling(); //<--adds support for Job Router SDK events

builder.Services.AddSingleton<RouterClient>(_ =>
         new RouterClient(builder.Configuration.GetSection("RouterClientConnectionString")["Key"]));
builder.Services.AddSingleton<RouterAdministrationClient>(_ =>
    new RouterAdministrationClient(builder.Configuration.GetSection("RouterClientConnectionString")["Key"]));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/EventGridEvents", (EventGridEvent[] eventGridEvents,
              [FromServices] IEventPublisher<Router> publisher) => publisher.Publish(eventGridEvents));

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.UseCosmosDb();

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}