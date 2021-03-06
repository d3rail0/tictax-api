using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using entities;
using Microsoft.EntityFrameworkCore;
using entities.Repositories.Interfaces;
using entities.Repositories;
using tictax.api.Services.Interfaces;
using tictax.api.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.IO;

namespace tictax.api
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
            services
                .AddCors(options =>
                {
                    options.AddPolicy("AllowAny", builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                });

            services.AddHttpContextAccessor();

            // Setup connection to the database
            services.AddDbContext<AppDbContext>(opts =>
             opts.UseSqlServer(Configuration["MyDatabase:DefaultConnection"],
                options => options.MigrationsAssembly("tictax.api")));

            // Register repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IProfileActivityRepository, ProfileActivityRepository>();

            // Register services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMatchService, MatchService>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "tictax.api", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Authorization: (\"bearer {token}\")",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            if (Program.IsStartedWithMain)
            {
                string plain_text_key = Environment.GetEnvironmentVariable("TICTAX_JWT_KEY");

                if (plain_text_key == null)
                {
                    throw new Exception("JWT key not set!");
                }

                byte[] sym_key = Encoding.UTF8.GetBytes(plain_text_key);

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(sym_key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            }
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "tictax.api v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                //OnPrepareResponse = ctx =>
                //{
                //    string[] allowedAnonymousFiles = { "index.html", "register.html", "auth.js", "authStyle.css" };

                //    // Allow all js and css libs
                //    if (ctx.File.PhysicalPath.Contains("\\wwwroot\\lib\\"))
                //    {
                //        return;
                //    }

                //    // Allow access to all images
                //    if (ctx.File.PhysicalPath.Contains("\\wwwroot\\img\\"))
                //    {
                //        return;
                //    }

                //    if (allowedAnonymousFiles.Contains(ctx.File.Name))
                //    {
                //        return;
                //    }

                //    if (ctx.Context.User.Identity.IsAuthenticated)
                //    {
                //        return;
                //    }

                //    ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //    ctx.Context.Response.ContentLength = 0;
                //    ctx.Context.Response.Body = Stream.Null;
                //}
            });

            app.UseDefaultFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            using (var scope = app.ApplicationServices.CreateScope())
            {
                // Since all active matches will be killed after server restarts
                // we will also remove all data from table Match to avoid having "zombie" matches
                var dbCtx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbCtx.Database.ExecuteSqlRaw("TRUNCATE TABLE [Match]");
            }

        }
    }
}
