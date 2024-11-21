using KmlApiProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp => new KmlDataService("wwwroot/DIRECIONADORES1.kml"));
builder.Services.AddSingleton<KmlExportService>();

builder.Services.AddEndpointsApiExplorer(); // Adiciona suporte para a explora��o de endpoints
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// Configura��o de middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
