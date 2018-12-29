using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using ResourceAPI.Mappers;
using ResourceAPI.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace ResourceAPI
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
            var connection = "Data Source=localhost;Initial Catalog=UserAuth;User ID=sa;Password=Zbj123";
            services.AddDbContext<Entities.UserContext>(options => options.UseSqlServer(connection));
            // Add framework services.

            //Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "资源服务器API"
                });

                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "ResourceAPI.xml");
                c.IncludeXmlComments(xmlPath);

                //  c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });

            //protect API
            services.AddMvcCore()
            .AddAuthorization()
            .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "api1";
                });

            //JS-allow Ajax calls to be made from http://localhost:5003 to http://localhost:5001.
            services.AddCors(options =>
            {
                //this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("http://localhost:5003")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //AddSwagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "资源服务器API");
            });

            //JS-Add the CORS middleware to the pipeline in Configure:
            app.UseCors("default");

            InitDataBase(app);
            app.UseAuthentication();
            //app.UseIdentityServer();
            app.UseMvc();
        }

        public void InitDataBase(IApplicationBuilder app)
        {

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<Entities.UserContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<Entities.UserContext>();
                context.Database.Migrate();
                if (!context.Users.Any())
                {
                    User user = new User()
                    {
                        UserId = "1",
                        UserName = "zhubingjian",
                        Password = "123",
                        IsActive = true,
                        Claims = new List<Claims>
                        {
                            new Claims("role","admin")
                        }
                    };
                    context.Users.Add(user.ToEntity());
                    context.SaveChanges();
                }
            }
        }
    }
}
