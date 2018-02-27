using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using senai.twitter.repository.Context;

namespace senai.twitter.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ambiente = BuildWebHost(args);
            using (var escopo = ambiente.Services.CreateScope())
            {
                var servico = escopo.ServiceProvider;
                try
                {
                    var contexto = servico.GetRequiredService<BikeMobiContext>();
                    IniciarBanco.Inicializar(contexto);
                }
                catch (Exception e)
                {
                    var logger = servico.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "Ocorreu um erro ao inserir dados.");
                }
            }

            ambiente.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
