using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.Common;
using Redis.Common.Abstractions;
using Redis.Common.Implementations;
using Redis.Master.Application;
using Redis.Master.Infrastructure;

namespace Redis.Master
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
            services.AddSwaggerGen();
            services.AddControllers();

            services.AddSingleton<IHashGenerator, JenkinsHashGenerator>();
            services.AddSingleton<IBinarySerializer, JsonSerializer>();
            services.AddSingleton<IBitHelper, BitHelper>();
            services.AddSingleton<IPrimeNumberService, PrimeNumberService>();
            services.AddSingleton<IMasterService, MasterService>();
            services.AddHttpClient<IChildClient, ChildClient>();
            services.AddHttpClient<IMasterClient, MasterClient>();
            services.AddSingleton<IReplicationService, ReplicationService>();
            services.Configure<MasterOptions>(Configuration.GetSection(nameof(MasterOptions)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
