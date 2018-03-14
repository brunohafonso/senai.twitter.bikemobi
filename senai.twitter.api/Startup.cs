using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;
using senai.twitter.repository.Context;
using senai.twitter.repository.Repositories;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

namespace senai.twitter.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                builder => builder.AllowAnyOrigin());
            });

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations")).Configure(tokenConfigurations);

            services.AddSingleton(tokenConfigurations);
            services.AddAuthentication(authOptions => { 
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions => {
                var parametrosValidacao = bearerOptions.TokenValidationParameters;
                parametrosValidacao.IssuerSigningKey = signingConfigurations.Key;
                parametrosValidacao.ValidAudience = tokenConfigurations.Audience;
                parametrosValidacao.ValidateIssuerSigningKey = true;
            });

            services.AddAuthorization(auth => {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
            });


            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {
                    Version = "V1",
                    Title = "BikeMobi API",
                    Description = "Documentação de uso da BikeMobi API",
                    TermsOfService = "None",
                    Contact = new Contact{Name = "Bruno Afonso", Email = "brunohafonso@gmail.com", Url = "https://www.linkedin.com/in/bruno-henrique-afonso-6028a4149/"}
                });

                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "bikeMobiApi.xml");

                c.IncludeXmlComments(xmlPath);
            });
            
            
            services.AddDbContext<BikeMobiContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BikeMobiContext")));
            
            services.AddMvc().AddJsonOptions(options => {
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
             });;

             services.AddScoped(typeof(IBaseRepository<>),typeof(BaseRepository<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseCors("AllowAnyOrigin");
                
                app.UseMvc();

                app.UseSwagger();

                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }
        }
    }
