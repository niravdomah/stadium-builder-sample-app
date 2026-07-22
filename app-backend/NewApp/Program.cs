using Twenty57.Builder.ApplicationRuntime.Interfaces;
using Twenty57.Builder.ApplicationRuntime.Sdk;
using NewApp;
using NewApp.Services.ApiHost;

var builder = Host.CreateApplicationBuilder();

var appSettings = builder.Configuration.Get<AppSettings>()!;
builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddSingleton(appSettings);

builder.Services.AddSingleton<Service>();
builder.Services.AddSingleton<IService>(provider => provider.GetRequiredService<Service>());

builder.Services.AddHostedService<Application>();

await Runner.RunAsync(builder.Build()).ConfigureAwait(false);
