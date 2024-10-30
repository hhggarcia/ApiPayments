using BncPayments.Middleware;
using BncPayments.Services;
using ClassLibrary.BncModels;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBncSettings>(builder.Configuration.GetSection("ApiBncSettings"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ApiBncSettings>>().Value);
builder.Services.AddSingleton<WorkingKeyServices>();

builder.Services.AddScoped<IBncServices, BncServices>();
builder.Services.AddTransient<IHashService, HashServices>();
builder.Services.AddTransient<IEncryptionServices, EncryptionServices>();

builder.Services.AddHostedService<WorkKeyUpdateServices>();

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();