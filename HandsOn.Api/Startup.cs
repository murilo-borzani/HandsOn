using HandsOn.Regras;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;

namespace HandsOn.Api
{
    public static class Startup
    {
        public static void RegistraServicos(this IServiceCollection services, ConfigurationManager configuration)
        {
            RegistraDependencias(services);
            services.RegistraMapeamentos();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddApiVersioning()
                            .AddMvc()
                            .AddApiExplorer(
                                 options => options.GroupNameFormat = "'v'VVV");

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations(true, true);
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Hands On API",
                    Description = "Esta API tem como finalidade o suporte as aplicações de planos de contas",
                    Contact = new OpenApiContact()
                    {
                        Name = "Murilo Borzani Lopes"
                    }
                });

                foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
                    c.IncludeXmlComments(file);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Token JWT",
                    Name = "Autorização",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

            services.AddHttpClient();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Auth:Chave"]!)),
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidAudience = configuration["Auth:Audience"],
                        ValidIssuer = configuration["Auth:Issuer"]
                    };
                });

            services.AddAuthorization();
        }

        public static void RegistraDependencias(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<Repositorio.IPlanoContasRepositorio, Repositorio.PlanoContasRepositorio>();
            services.AddScoped<Regras.IPlanoContasServico, Regras.PlanoContasServico>();

            services.AddScoped<Regras.ISegurancaServico, Regras.SegurancaServico>();
            services.AddScoped<Repositorio.IUsuarioRepositorio, Repositorio.UsuarioRepositorio>();
        }

        public static void ConfiguraApp(this WebApplication app)
        {
            app.MapDefaultEndpoints();

            var supportedCultures = new[] { new CultureInfo("pt-BR") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    foreach (var description in app.DescribeApiVersions())
                    {
                        var url = $"/swagger/{description.GroupName}/swagger.json";
                        var name = description.GroupName.ToUpperInvariant();
                        opt.SwaggerEndpoint(url, name);
                    }
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
