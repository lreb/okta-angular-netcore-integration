using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Okta.AspNetCore;

namespace okta_integration
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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
                // The CORS policy is open for testing purposes. In a production application, you should restrict it to known origins.
                options.AddPolicy(
                    "AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });
            // services.AddCors(options => {
            //      options.AddPolicy(
            //         "Local",
            //         builder => builder.WithOrigins("http://localhost:4200/")
            //                           .AllowAnyMethod()
            //                           .AllowAnyHeader());
            // });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
            })
            .AddOktaWebApi(new OktaWebApiOptions()
            {
                OktaDomain = Configuration["Okta:OktaDomain"]
            });
            
            
            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = OktaDefaults.MvcAuthenticationScheme;
            // })
            // .AddCookie()
            // .AddOktaMvc(new OktaMvcOptions
            // {
            //     // Replace these values with your Okta configuration
            //     OktaDomain = Configuration["Okta:OktaDomain"],                
            //     ClientId = Configuration["Okta:ClientId"],
            //     ClientSecret = Configuration["Okta:ClientSecret"]
            // });

            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // .AddJwtBearer(options =>
            // {
            //     options.Authority = Configuration["Okta:OktaDomain"];
            //     options.Audience = "api://default";
            // });
            // services.AddCors(options =>
            // {
            //     options.AddPolicy(name: oktaSpecificOrigins,
            //                     builder =>
            //                     {
            //                         builder.WithOrigins("http://localhost:4200");
            //                     });
            // });
            // services.AddCors(options =>
            // {
            //     options.AddPolicy(name: okta,
            //                     builder =>
            //                     {
            //                         builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //                     });
            // });

            // services.AddAuthorization();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseCors("Local");
            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // .RequireCors(okta);
            });
        }
    }
}
