using SprintQuiz.Api.Extensions;
using SprintQuiz.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services
builder.Services.AddControllers();

// Services SprintQuiz
builder.Services.AddSprintQuizServices(builder.Configuration);

// Authentification JWT
builder.Services.AddSprintQuizAuthentication(builder.Configuration);

// CORS
builder.Services.AddSprintQuizCors();

// Swagger/OpenAPI
builder.Services.AddSprintQuizSwagger();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SprintQuiz API V1");
        c.RoutePrefix = string.Empty; // Swagger UI à la racine
    });
}

// Middleware personnalisé de gestion des erreurs
app.UseExceptionHandlingMiddleware();

// CORS
app.UseCors("AllowAll");

// HTTPS Redirection
app.UseHttpsRedirection();

// Servir les fichiers statiques (pour les photos de profil)
app.UseStaticFiles();

// Authentification et autorisation
app.UseAuthentication();
app.UseAuthorization();

// Mapping des contrôleurs
app.MapControllers();

// Point de santé de l'API
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Features = new
    {
        Authentication = "JWT",
        Database = "PostgreSQL",
        FileUpload = "Enabled"
    }
});

// Message de démarrage
app.Logger.LogInformation("SprintQuiz API démarrée avec succès avec authentification JWT");

app.Run();

