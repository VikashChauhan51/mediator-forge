using Akka.Actor;
using Akka.DependencyInjection;
using MediatorApi;
using MediatorApi.Commands;
using MediatorForge;
using MediatorForge.CQRS.Validators;
using System.Diagnostics;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddProblemDetails(options =>
{
    // Customize ProblemDetails
    options.CustomizeProblemDetails = (context) =>
    {
        var problemDetails = context.ProblemDetails;
        context.HttpContext.Response.ContentType = "application/problem+json";
        problemDetails.Instance = $"{context.HttpContext?.Request.Method} {context.HttpContext?.Request.Path}";
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext?.TraceIdentifier;

    };
});


// Register Akka.NET actor system
builder.Services.AddSingleton(provider =>
{
    var bootstrap = BootstrapSetup.Create();
    // enable DI support inside this ActorSystem, if needed
    var diSetup = DependencyResolverSetup.Create(provider);
    // merge this setup (and any others) together into ActorSystemSetup
    var actorSystemSetup = bootstrap.And(diSetup);
    var actorSystem = ActorSystem.Create("MediatorForge", actorSystemSetup);
    return actorSystem;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatorForge(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<IValidator<CreateItemCommand>, CreateItemCommandValidator>();
builder.Services.AddTransient<IValidator<CreateItemOptionCommand>, CreateItemOptionCommandValidator>();
builder.Services.AddTransient<IValidator<CreateItemResultCommand>, CreateItemResultCommandValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
