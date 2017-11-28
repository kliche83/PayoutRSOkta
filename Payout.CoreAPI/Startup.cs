using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Payout.CoreAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using System;

namespace Payout.CoreAPI
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
            services.Configure<AppSettingsCls.ConfigJwtSecurityToken>(
                Configuration.GetSection("JwtSecurityToken"));

            services.Configure<AppSettingsCls.URLKeys>(
                Configuration.GetSection("URLKeys"));

            services.Configure<AppSettingsCls.OktaKeys>(
                Configuration.GetSection("Okta"));

            services.Configure<AppSettingsCls.MailCredentials>(
                Configuration.GetSection("MailCredentials"));

            // Add framework services.

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityToken:Key"]));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = Configuration.GetSection("JwtSecurityToken:Issuer").Value,
                ValidAudience = Configuration.GetSection("JwtSecurityToken:Audience").Value,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = signingKey,
                ValidateLifetime = true
            };


            services.AddIdentity<PayoutUser, PayoutRole>(
                    options =>
                    {//THIS IS REQUIRED TO AUTOGENERATE DEFAULT TOKEN FOR RESET PASSWORD
                        //h_ttps://stackoverflow.com/questions/40445980/password-reset-token-provider-in-asp-net-core-iusertokenprovider-not-found
                        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                    }
                ).AddEntityFrameworkStores<SecurityContext>()
                .AddDefaultTokenProviders();
            
            services.AddAuthentication(sharedOptions =>
            {
                //sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                // Configuration pulled from appsettings.json by default:
                options.ClientId = Configuration["okta:ClientId"];
                options.ClientSecret = Configuration["okta:ClientSecret"];
                options.Authority = Configuration["okta:Authority"];
                options.CallbackPath = "/authorization-code/callback";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.UseTokenLifetime = false;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
            });

            services.AddMvc();
            services.AddCors();
            SetUpDataBase(services);
        }

        public virtual void SetUpDataBase(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<SecurityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SecurityConnection"), sqlOptions => sqlOptions.MigrationsAssembly("Payout.CoreAPI")));
        }

        public virtual void EnsureDatabaseCreated(SecurityContext dbContext)
        {
            // run Migrations            
            dbContext.Database.Migrate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            /*THIS ORDER IS VEEERY IMPORTANT - APP.USECORS HAS TO BE FIRST AND THEN APP.USEMVC*/
            app.UseCors(corsPolicyBuilder =>
               //corsPolicyBuilder.WithOrigins("h_ttp://localhost:51327") //WebForms project
               corsPolicyBuilder.WithOrigins(Configuration.GetSection("URLKeys:ClientURL").Value) //WebForms project
              .AllowAnyMethod()
              .AllowAnyHeader()
            );

            //app.UseMvc();
            app.UseMvc(routes =>
            {
                /*THIS IS REQUIRED FOR OKTA -- must have to find how to deal with this in api instead of razor*/
                
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Default}/{action=Index}/{id?}");
            });

            // within your Configure method:
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
              .CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<SecurityContext>();
                EnsureDatabaseCreated(dbContext);
            }
        }
    }
}
