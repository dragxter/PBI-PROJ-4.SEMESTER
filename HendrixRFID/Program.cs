using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using HendrixRFID.Data;
using HendrixRFID.DTOs;
using HendrixRFID.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hendrix.db"));

// Channel — køen mellem MQTT-lytter og processor
var channel = Channel.CreateBounded<MqttScanMessage>(500);
builder.Services.AddSingleton(channel);

// Services
builder.Services.AddSingleton<EartagDecoder>();
builder.Services.AddScoped<PigService>();
builder.Services.AddScoped<PigLocationService>();

// Background services
builder.Services.AddHostedService<MqttListenerService>();
builder.Services.AddHostedService<ScanProcessorService>();

// REST API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();