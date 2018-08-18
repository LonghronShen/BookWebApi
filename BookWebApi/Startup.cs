#define USE_PostgreSQL

using BookWebApi.Models;
using BookWebApi.Utilities;
using idunno.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookWebApi
{

    public class Startup
    {

        public IConfiguration Configuration { get; }

        public string UserName { get; }

        public string Password { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
            this.UserName = this.Configuration["Auth:UserName"];
            this.Password = this.Configuration["Auth:Password"];
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<BookDbContext>(options =>
                {
#if USE_PostgreSQL
                    var connectionString = this.Configuration.GetConnectionString("PostgreSQL");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        options.UseNpgsql(connectionString);
                    }
#else
                    var connectionString = this.Configuration.GetConnectionString("MySql");
					if (!string.IsNullOrEmpty(connectionString))
					{
						options.UseMySql(connectionString);
					}
#endif
                    else
                    {
                        options.UseInMemoryDatabase("Book");
                    }
                }, ServiceLifetime.Scoped);

            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
               .AddBasic(options =>
               {
                   options.AllowInsecureProtocol = true;
                   options.Realm = "idunno";
                   options.Events = new BasicAuthenticationEvents
                   {
                       OnValidateCredentials = context =>
                       {
                           if (context.Username == this.UserName && context.Password == this.Password)
                           {
                               var claims = new[]
                               {
                                    new Claim(ClaimTypes.NameIdentifier, context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                               };
                               context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                               context.Success();
                           }
                           return Task.CompletedTask;
                       }
                   };
               });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AlwaysFail", policy => policy.Requirements.Add(new AlwaysFailRequirement()));
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddSwaggerGen(c =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1", new Info { Title = "Book API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseStatusCodePages();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                     name: "default",
                     template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api-docs/{documentName}/swagger.json";
                options.PreSerializeFilters.Add((document, request) =>
                {
                    document.Host = request.Host.Value;
                });
            });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api-docs";
                c.SwaggerEndpoint("/api-docs/v1/swagger.json", "Book API V1");
                c.DocExpansion(DocExpansion.Full);
                c.DisplayRequestDuration();
                c.ShowExtensions();
                c.EnableValidator();
            });
        }

    }

}