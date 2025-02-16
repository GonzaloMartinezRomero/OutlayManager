using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OutlayManager.Infraestructure;
using System;
using System.IO;
using System.Reflection;

namespace OutlayManagerAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private const string URL_CORS_HOST_KEY = "HostWeb";
        private const string OUTLAY_MANAGER_CORS_POLICY = "OutlayManagerWebClientCors";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .ConfigureApiBehaviorOptions(options=> 
                    {   
                        options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
                    });

            services.AddInfraestructure(Configuration);
          
            services.AddCors(corsSetting => corsSetting.AddPolicy(OUTLAY_MANAGER_CORS_POLICY, policy =>
            {
                policy.WithOrigins(Configuration[URL_CORS_HOST_KEY]);
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            }));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Outlay API Documentation", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {   
            app.UseRouting();
            app.UseCors(OUTLAY_MANAGER_CORS_POLICY);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Swagger configuration
            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Outlays Manager API Documentation");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
