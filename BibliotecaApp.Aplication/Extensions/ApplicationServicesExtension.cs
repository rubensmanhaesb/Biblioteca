using BibliotecaApp.Aplication.Interfaces;
using BibliotecaApp.Aplication.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<ILivroAppService, LivroAppService>();
            services.AddTransient<IAutorAppService, AutorAppService>();
            services.AddTransient<IAssuntoAppService, AssuntoAppService>();
            services.AddTransient<ILivroAutorAppService, LivroAutorAppService>();
            services.AddTransient<ILivroAssuntoAppService, LivroAssuntoAppService>();
            services.AddTransient<IPrecoLivroAppService, PrecoLivroAppService>();

            return services;
        }
    }
}
