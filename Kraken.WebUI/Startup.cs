using Kraken.Application.Models;
using Kraken.Application.Models.Mappers;
using Kraken.Application.Services.Implementation;
using Kraken.Application.Services.Interfaces;
using Kraken.Calculation;
using Kraken.Calculation.Field;
using Kraken.Calculation.Field.Interfaces;
using Kraken.Calculation.Interfaces;
using Kraken.Calculation.Models;
using Kraken.Common.Mappers;
using Kraken.WebUI.Models;
using Kraken.WebUI.Models.Common;
using Kraken.WebUI.Models.Mappers;
using Kraken.WebUI.Models.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kraken.WebUI
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

            services.AddControllersWithViews().AddNewtonsoftJson();
         
            services.AddTransient<IModelValidator<KrakenInputModel>, KrakenInputModelValidator>();
            services.AddTransient<IMapper<KrakenInputModel, AcousticProblemData>, KrakenInputModelMapper>();
            services.AddTransient<IMapper<KrakenComputingResult, KrakenResultModel>, KrakenResultModelMapper>();

            services.AddTransient<IKrakenService, KrakenService>();          
            services.AddTransient<KrakenComputingResultMapper>();

            services.AddTransient<IKrakenNormalModesProgram, KrakenNormalModesProgram>();
            services.AddTransient<IFieldProgram, FieldProgram>();

            services.AddTransient<IMapper<AcousticProblemData, KrakenInputProfile>, KrakenInputProfileMapper>();
            services.AddTransient<IMapper<FieldComputingRequiredData, FieldInputData>, FieldInputDataMapper>();
            services.AddTransient<IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult>, KrakenComputingResultMapper>();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
