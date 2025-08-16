using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Repositories;
using SprintQuiz.Api.Services;
using SprintQuiz.Data.Repositories;
using SprintQuiz.Services;
using System.Text;

namespace SprintQuiz.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSprintQuizServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration de la base de données
            services.AddDbContext<SprintQuizDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Enregistrement des repositories
            services.AddRepositories();

            // Enregistrement des services
            services.AddBusinessServices();

            // Configuration d'AutoMapper
            services.AddAutoMapper(typeof(Program));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IFormationRepository, FormationRepository>();
            services.AddScoped<ISprintRepository, SprintRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<ICoursRepository, CoursRepository>();
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();

            return services;
        }

        private static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<IFormationService, FormationService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ICoursService, CoursService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IExerciceService, ExerciceService>();
            services.AddScoped<IUtilisateurService, UtilisateurService>();
            services.AddScoped<IQAQuestionService, QAQuestionService>();
            services.AddScoped<IProfilService, ProfilService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        public static IServiceCollection AddSprintQuizAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey n'est pas configurée dans appsettings.json");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("EtudiantOrAdmin", policy => policy.RequireRole("Etudiant", "Admin"));
            });

            return services;
        }

        public static IServiceCollection AddSprintQuizCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IServiceCollection AddSprintQuizSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "SprintQuiz API",
                    Version = "v1",
                    Description = "API pour la plateforme de révision SprintQuiz avec authentification JWT",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Équipe SprintQuiz"
                    }
                });

                // Configuration pour JWT dans Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Inclure les commentaires XML pour la documentation
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }
    }
}

