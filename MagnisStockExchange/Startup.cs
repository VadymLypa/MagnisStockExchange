using System;
using MagnisStockExchange.Abstractions;
using MagnisStockExchange.Services;
using MagnisStockExchange.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
namespace MagnisStockExchange
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
            services.AddWebSocketManager();
            services.AddMvc();
            services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc("v1", new Info { Title = "Magnis Stock Exchange", Description = "Technical Task" });
           });
            
            services.AddTransient<IRandomGenerateValue, RandomGenerateService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var wsOption = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(wsOption);
            app.MapWebSocketManager("/chat", serviceProvider.GetService<RealTimeDataService>());
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {   
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Magnis Stock Exchange");
            });
        }
    }
}
