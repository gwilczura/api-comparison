using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Wilczura.Demo.Host.Controllers;
using Wilczura.Demo.Host.Setup;
using Wilczura.Demo.Persistence;

var builder = WebApplication.CreateBuilder(args);

var domainName = "demo";
var logger = builder.GetStartupLogger(domainName);

builder.AddCommonServices(domainName, logger);
var controllersAssemblyPart = new AssemblyPart(typeof(HealthController).Assembly);
var mvcBuilder = builder.AddWebServices(controllersAssemblyPart, logger);
mvcBuilder.AddOData(builder, logger);
builder.AddOtherServices();


builder.AddPostgres<DemoDbContext>(connectionStringName: domainName, logger: logger, sectionName: null);
var app = builder.Build();

await app.ApplyDatabaseMigrationsAsync<DemoDbContext>();

app.UseDefaults();

app.Run();
