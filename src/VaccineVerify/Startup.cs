using VaccineVerify.Data;
using VaccineVerify.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;

namespace VaccineVerify
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.Configure<MattrConfiguration>(Configuration.GetSection("MattrConfiguration"));
            services.AddScoped<MattrTokenApiService>();
            services.AddScoped<MattrPresentationTemplateService>();
            services.AddScoped<MattrCredentialVerifyCallbackService>();
            services.AddScoped<VaccineVerifyDbService>();
            services.AddScoped<MattrCreateDidService>();

            services.AddDbContext<VaccineVerifyVerifyMattrContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddRazorPages();
            services.AddSignalR();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // we need to diable this so ngrok works for local dev without a license
            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<MattrVerifiedSuccessHub>("/mattrVerifiedSuccessHub");
                endpoints.MapControllers();
            });
        }
    }
}
