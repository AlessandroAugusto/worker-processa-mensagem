using Mensagem.Domain.Interfaces;
using Mensagem.Infra.Data.Contexts;
using Mensagem.Infra.Data.Filas;
using Mensagem.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Diagnostics;


namespace Mensagem.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<DbConnectionStringBuilder>(new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                ConnectionString = ""
            });

            services.AddSingleton<DbConnectionStringBuilder>(new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                ConnectionString = ""
            });

            services.AddSingleton<DatabaseContext>();

            //Services
            services.AddTransient<IRabbitMQHelper, RabbitMqHelper>();
            services.AddTransient<IProcessaFilaService, ProcessaFilaService>();

            return services;
        }
    }
}
