#region Usings

using ApplicationLayer.Behaviors;
using ApplicationLayer.Common.Validations;
using ApplicationLayer.Extensions;
using ApplicationLayer.MapperProfile.UserAccounts;
using AspNetCoreRateLimit;
using DomainLayer.Common.Attributes;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Telegram.Bot;

#endregion

namespace ApplicationLayer
{
    public static class IoCContainer
    {
        public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterService(configuration);
            services.AddSingleton<IApplicationBuilder, ApplicationBuilder>();
            services.AddHttpContextAccessor();
            services.MediatRDependency();
            services.RegisterServicesAutomatically();
            services.FluentValidationConfiguration();
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
            services.RateLimitingConfiguration(configuration);
            services.AddCors(opt => opt.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.WithOrigins("https://tg.packsi.net", "https://api.packsi.net", "https://panel.packsi.net", "http://localhost:3000", "http://localhost:5173", "http://localhost:8080", "http://127.0.0.1:3000", "http://127.0.0.1:5173", "http://127.0.0.1:8080")
                .SetIsOriginAllowed(origin => true) // Allow any origin for development
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            }));

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365); // 1 year
            });
            return services;
        }

        private static void RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigurationDependency();
            services.AddMemoryCache();
            services.SwaggerConfiguration(configuration);
            services.JwtAuthorizeConfiguration(configuration);
            services.SeriLogConfiguration(configuration);
            services.AddAutoMapper(typeof(UserAccountProfile).GetTypeInfo().Assembly);
        }

        private static void SeriLogConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var columnOptions = new ColumnOptions();
            // Define additional custom columns if needed
            //columnOptions.AdditionalColumns = new Collection<SqlColumn>
            //{
            //    new SqlColumn { ColumnName = "CustomColumn1", DataType = System.Data.SqlDbType.NVarChar, DataLength = 100 },
            //    new SqlColumn { ColumnName = "CustomColumn2", DataType = System.Data.SqlDbType.Int }
            //};

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: configuration.GetConnectionString("LogConnection"),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = configuration.GetSection("Serilog:TableName").Value,
                        SchemaName = configuration.GetSection("Serilog:SchemaName").Value,
                        AutoCreateSqlTable = true
                    },
                    columnOptions: columnOptions
                )
                .CreateLogger();

            services.AddSingleton(Log.Logger);
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
        }

        private static void SwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                options.SwaggerDoc("Identity", new OpenApiInfo { Title = "API Identity", Version = "v1" });
                options.SwaggerDoc("Administrator", new OpenApiInfo { Title = "API Administrator", Version = "v1" });
                options.SwaggerDoc("Managers", new OpenApiInfo { Title = "API Managers", Version = "v1" });
                options.SwaggerDoc("Users", new OpenApiInfo { Title = "API Users", Version = "v1" });
                options.SwaggerDoc("MiniApp", new OpenApiInfo { Title = "API MiniApp", Version = "v1" });
            });
        }

        private static void JwtAuthorizeConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new ArgumentException("JWT Key is not configured");

            var Key = Encoding.UTF8.GetBytes(jwtKey);

            // Ensure key is at least 256 bits (32 bytes) for security
            if (Key.Length < 32)
                throw new ArgumentException("JWT key must be at least 256 bits (32 bytes) long for security");

            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.TokenValidationParameters = tokenValidationParameter;

                option.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                        // Map custom role claim if necessary
                        var roleClaims = claimsIdentity.FindAll("role").ToList();
                        foreach (var roleClaim in roleClaims)
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSingleton(tokenValidationParameter);
        }

        private static void FluentValidationConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddValidatorsFromAssemblyContaining<RefreshTokensValidator>();
        }

        private static void ConfigurationDependency(this IServiceCollection services)
        {
            services.AddHttpClient();
        }

        private static void RateLimitingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure IP rate limiting
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // Configure client rate limiting
            services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

            // Add rate limiting services
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        private static void MediatRDependency(this IServiceCollection services)
        {
            services.AddMediatR(m =>
            {
                m.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                // Register handlers from PresentationApp assembly for domain events
                var presentationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "PresentationApp");
                if (presentationAssembly != null)
                {
                    m.RegisterServicesFromAssembly(presentationAssembly);
                }
            })
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }

        public static void RegisterServicesAutomatically(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Register non-generic types
            var nonGenericTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                && (t.GetCustomAttribute<InjectAsScopedAttribute>() != null ||
                t.GetCustomAttribute<InjectAsTransientAttribute>() != null ||
                t.GetCustomAttribute<InjectAsSingletonAttribute>() != null)
                && t.GetInterfaces().Any(i => !i.IsGenericType))
                .Select(t => new
                {
                    ServiceType = t.GetInterfaces().FirstOrDefault(),
                    ImplementationType = t,
                    Scoped = t.GetCustomAttribute<InjectAsScopedAttribute>() != null,
                    Transient = t.GetCustomAttribute<InjectAsTransientAttribute>() != null,
                    Singleton = t.GetCustomAttribute<InjectAsSingletonAttribute>() != null
                }).Where(x => x.ServiceType != null);

            foreach (var type in nonGenericTypes)
            {
                if (type.Scoped)
                {
                    services.AddScoped(type.ServiceType, type.ImplementationType);
                }
                else if (type.Transient)
                {
                    services.AddTransient(type.ServiceType, type.ImplementationType);
                }
                else if (type.Singleton)
                {
                    services.AddSingleton(type.ServiceType, type.ImplementationType);
                }
            }
        }

        public static void RegisterTelegram(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("8109507045:AAG5iY_c1jLUSDeOOPL1N4bnXPWSvwVgx4A")
);
        }
    }
}