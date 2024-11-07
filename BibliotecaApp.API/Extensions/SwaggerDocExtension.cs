using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BibliotecaApp.API.Extensions
{
    public static class SwaggerDocExtension
    {

        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            // Configurações adicionais do Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Desafio Técnico",
                    Version = "v1",
                    Description = "API Desafio",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Rubens Bernardes",
                        Email = "rubensmanhaesb@hotmail.com",
                        Url = new Uri("https://www.tjrj.jus.br/")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Configuração do JWT Bearer no Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Por favor, insira o token JWT Bearer",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
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
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "BibliotecaApp");
            });

            return app;
        }
    }
}
