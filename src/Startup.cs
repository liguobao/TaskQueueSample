using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using MTQueue.Listener;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTQueue.Model;
using MTQueue.Service;
using StackExchange.Redis;

namespace MTQueue
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCorrelationId();
            services.AddOptions().Configure<AppSettings>(Configuration);
            services.AddScoped<RequestService, RequestService>();
            services.AddSingleton<ZhihuClient, ZhihuClient>();

            InitRedis(services);
            services.AddHostedService<RedisMQListener>();
        }

        private void InitRedis(IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexer, ConnectionMultiplexer>(factory =>
            {
                ConfigurationOptions options = ConfigurationOptions.Parse(Configuration["RedisConnectionString"]);
                options.SyncTimeout = 10 * 10000;
                return ConnectionMultiplexer.Connect(options);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "x-request-id",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false
            });
            app.UseMvc();



        }
    }
}
