using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PLNKTNv2.BusinessLogic.Authentication;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;
using PLNKTNv2.BusinessLogic.Helpers;
using PLNKTNv2.BusinessLogic;
using PLNKTNv2.Persistence.Repositories;
using PLNKTNv2.Persistence.Repositories.Implementation;
using PLNKTNv2.Persistence;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.BusinessLogic.Services.Implementation;
using PLNKTNv2.BusinessLogic.Helpers.Implementation;

namespace PLNKTNv2
{
    public class Startup
    {
        // TODO:  Why is this public?  Is it needed?  No it's part of S3ProxyController.
        public const string AppS3BucketKey = "AppS3Bucket";

        // TODO: Need to be added as env vars
        private readonly string _cognitoAuthority = "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_yt7xxSRrl";
        private readonly string _cognitoAudience = "4s9n3lg2ptogi5o4pj899332kj";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add JWT Authentication and Policy based Authorisation
            services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.Audience = _cognitoAudience;
                options.Authority = _cognitoAuthority;
            });

            // Add custom authorization handlers with DI code
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EndUser", policy => policy.Requirements.Add(new CustomRoleRequirement("EndUser")));
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new CustomRoleRequirement("Admin")));
            });
            services.AddSingleton<IAuthorizationHandler, CustomRoleAuthorizationHandler>();

            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "PLNKTN API", Version = "v2" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            // Add custom DI code
            // Persistence
            services.AddTransient<IDbContextFactory, DbContextFactory>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRewardRegionService, RewardRegionService>();
            services.AddTransient<IGrantedRewardService, GrantedRewardService>();
            services.AddTransient<IEcologicalMeasurementService, EcologicalMeasurementService>();
            services.AddTransient<ICollectiveEfService, CollectiveEfService>();
            services.AddTransient<IBinService, BinService>();

            // Business Logic
            services.AddTransient<IAccount, Account>();
            services.AddTransient<IMessenger, Email>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v2/swagger.json", "PLNKTN API v2");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}