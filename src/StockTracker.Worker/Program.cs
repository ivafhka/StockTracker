using StockTracker.Infrastructure;
using StockTracker.Infrastructure.ExternalApis.Finnhub;
using StockTracker.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<FinnhubOptions>(
    builder.Configuration.GetSection(FinnhubOptions.SectionName));

builder.Services.AddSingleton<FinnhubWebSocketClient>();

builder.Services.AddHostedService<FakeQuoteWorker>();

var host = builder.Build();
host.Run();
