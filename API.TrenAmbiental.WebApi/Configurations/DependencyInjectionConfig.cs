using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.Data.Mysql.Repositories;
using API.TrenAmbiental.Domain.Services;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.TrenAmbiental.WebApi.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolverDependencias(this IServiceCollection services)
        {
            services.AddScoped<INotificador, Notificador>();

            services.AddScoped<IAutenticacaoService, AutenticacaoService>();
            services.AddScoped<IAutenticacaoRepository, AutenticacaoRepository>();
            services.AddScoped<ICadastroService, CadastroService>();
            services.AddScoped<ICadastroRepository, CadastroRepository>();
            services.AddScoped<IPesagemService, PesagemService>();
            services.AddScoped<IPesagemRepository, PesagemRepository>();
            services.AddScoped<IPontuacaoService, PontuacaoService>();
            services.AddScoped<IPontuacaoRepository, PontuacaoRepository>();
            services.AddScoped<ICatalogoService, CatalogoService>();
            services.AddScoped<ICatalogoRepository, CatalogoRepository>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}