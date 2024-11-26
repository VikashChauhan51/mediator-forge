using MediatorApi;
using MediatorApi.Commands;
using MediatorForge;
using MediatorForge.CQRS.Interfaces;
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
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

    };
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
