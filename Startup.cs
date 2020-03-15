using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Facebook
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddDbContextPool<AppDbContext>(option => option.UseSqlServer(Configuration.GetConnectionString("MyConnection")));


          //test Git :D
            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.SignIn.RequireConfirmedEmail = true;
             })
                .AddEntityFrameworkStores<AppDbContext>()
                  .AddDefaultTokenProviders();



            services.AddControllersWithViews();

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "1012776754691-4d28fl5qpp8cd4c88dudh5kio549cjcc.apps.googleusercontent.com";
                options.ClientSecret = "QQ8n40jDoimKKJ6FI12kGhzp";
             //   options.CallbackPath = new PathString("/signin-google/");
            }).AddFacebook(options => {
                options.ClientId = "228508618197951";
                options.ClientSecret = "fbff9e7c23cc0c428ab8207b7fb3319c";
                //   options.CallbackPath = new PathString("/signin-google/");
            });

            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
              //  app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
