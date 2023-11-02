using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;
using WebApiAutores.Middlewares;
using WebApiAutores.services;
using WebApiAutores.Services.ImplementServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutores.Filters;
using WebApiAutores.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace WebApiAutores
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opciones =>
            {
                /* Registro de un filtro de forma global */
                opciones.Filters.Add(typeof(MethodExceptionFilter));
            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

            /* servicio donde se configura el db context el cual apunta a defaulConnection que esta en el appsettings.json */
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            
            services.AddEndpointsApiExplorer();
            services.AddTransient<IServicio, ServicioA>();
            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            //servicio de Action Filter
            services.AddTransient<MethodFilterAction>();

            //servicio de cache
            services.AddResponseCaching();
            //servicios recurrentes
            services.AddHostedService<WriteFiles>();

            //servicio de autorizacion y configuracion de token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                    opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer= false,
                        ValidateAudience= false,
                        ValidateLifetime= true,
                        ValidateIssuerSigningKey= true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["KeyJWT"])
                        ),
                        ClockSkew=TimeSpan.Zero
                    }
                );

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "WebApiAutores", Version= "v1"
                    });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] {}
                        }
                    });
                }    
            );

            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("EsAdmin"));
            });

            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            app.UseLoggerResponseHttp();

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { 
                endpoints.MapControllers();
            });
        }
    }
}
