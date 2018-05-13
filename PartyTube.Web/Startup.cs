using System.IO;
using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PartyTube.DataAccess;
using PartyTube.Model.Settings;
using PartyTube.Web.Hubs;

namespace PartyTube.Web
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, PartyTubeDbContextSeedData seed)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    //app.UseExceptionHandler("/Home/Error");
            //    app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            //}

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PlayerHub>("/Hubs/PlayerHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });

            seed.SeedData();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var youtubeApiKey = Configuration["JProffPartyTube:YoutubeApiKey"];
            var applicationName = Configuration["AppSettings:ApplicationName"];
            builder.RegisterModule(new AutofacModule(youtubeApiKey, applicationName));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSignalR();
            services.AddAutoMapper();
            services.AddDbContext<PartyTubeDbContext>(
                options => options.UseSqlite(
                    $"Data Source={Path.Combine(Configuration["contentRoot"], Configuration["AppSettings:DbFile"])}"));

            services.Configure<AppSettings>(settings =>
            {
                Configuration.GetSection("AppSettings").Bind(settings);
                settings.YoutubeApiKey = Configuration["JProffPartyTube:YoutubeApiKey"];
            });

            services.AddScoped(sp => sp.GetService<IOptionsSnapshot<AppSettings>>().Value);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }
    }
}