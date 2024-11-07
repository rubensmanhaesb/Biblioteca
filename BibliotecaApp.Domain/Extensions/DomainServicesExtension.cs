using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Domain.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Extensions
{
    public static class DomainServicesExtension
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<IAssuntoDomainService, AssuntoDomainService>();

            services.AddTransient<IAutorDomainService, AutorDomainService>();

            services.AddTransient<ILivroDomainService, LivroDomainService>();

            services.AddTransient<ILivroAssuntoDomainService, LivroAssuntoDomainService>();
            services.AddTransient<IValidator<LivroAssunto>, LivroAssuntoValidator>();

            services.AddTransient<ILivroAutorDomainService, LivroAutorDomainService>();
            services.AddTransient<IValidator<LivroAutor>, LivroAutorValidator>();

            services.AddTransient<IPrecoLivroDomainService, PrecoLivroDomainService>();

            return services;
        }
    }
}
