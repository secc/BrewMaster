using BrewMasterWeb.Data;
using BrewMasterWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SECC.Rock;
using SECC.Rock.Configuration;
using SECC.Rock.Identity;

namespace BrewMasterWeb
{
    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddControllersWithViews();

            services.AddScoped<IContainerFactory, ContainerFactory>();
            services.AddScoped<ICoffeeMakerService, CoffeeMakerService>();

            services.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
                .AddCookie( options =>
                 {
                     options.LoginPath = "/auth/login";
                     options.LogoutPath = "/auth/logout";
                 } );

            services.AddAuthorization( options =>
            {
                options.AddPolicy( ClaimsPolicies.View, policy => policy.RequireClaim( "Role", ClaimsPolicies.View, ClaimsPolicies.Edit, ClaimsPolicies.Administrate ) );
                options.AddPolicy( ClaimsPolicies.Edit, policy => policy.RequireClaim( "Role", ClaimsPolicies.Edit, ClaimsPolicies.Administrate ) );
                options.AddPolicy( ClaimsPolicies.Administrate, policy => policy.RequireClaim( "Role", ClaimsPolicies.Administrate ) );
            } );

            services.AddRock();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler( "/Home/Error" );
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints( endpoints =>
             {
                 endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Home}/{action=Index}/{id?}" );
             } );
        }
    }
}
