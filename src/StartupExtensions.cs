using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Conduit
{
    public static class StartupExtensions
    {
        public static void AddJwt(this IServiceCollection services)
        {
            services.AddOptions();

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("TheLordIsmyShepherdPsalm23"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var issuer = "issuer";
            var audience = "audience";

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = signingCredentials;
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingCredentials.Key,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = issuer,
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = audience,
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = (context) =>
                        {
                            var token = context.HttpContext.Request.Headers["Authorization"];
                            if (token.Count > 0 && token[0].StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = token[0].Substring("Token ".Length).Trim();
                            }

                            return Task.CompletedTask;
                        }
                    };

                });
        }

        public static void AddSerilogLogging(this ILoggerFactory loggerFactory)
        {
            // Attach the sink to the logger configuration
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                //just for local debug
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            loggerFactory.AddSerilog(log);
            Log.Logger = log;
        }


        public static IWebHost SeedData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<ConduitContext>();
                var hasher = services.GetService<IPasswordHasher>();

                DataSeeder.Seed(context, hasher);
            }
            return host;
        }


        public static void AddSpa(this IApplicationBuilder app, 
            IHostingEnvironment env, 
            Action<SpaOptions> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var options = new SpaOptions();
            configuration.Invoke(options);

            // branch middleware to a app-specific path
            app.Map(options.MapPath, spaApp => {
                // we only need SPA static files in prod mode
                // don't use AddSpaStaticFiles -- it puts a Singleton file provider in the container
                var fileOptions = new StaticFileOptions();
                if (!env.IsDevelopment())
                {
                    // path should be dist folder of the SPA
                    // this will error if ng prod build has not been run
                    var staticPath = Path.Combine(Directory.GetCurrentDirectory(), $"{options.SourcePath}{options.DistPath}");
                    fileOptions.FileProvider = new PhysicalFileProvider(staticPath);

                    // this will root in the MapPath since we are branched
                    spaApp.UseSpaStaticFiles(options: fileOptions);
                }

                // create the SPA within the branch path
                spaApp.UseSpa(spa =>
                {
                    spa.Options.SourcePath = options.SourcePath;
                    spa.Options.DefaultPage = options.DefaultPage;


                    if (env.IsDevelopment())
                    {
                        // this defaults to start:hosted, which has some ng serve options for multi-spa
                        spa.UseAngularCliServer(npmScript: options.DevServerScript);
                    }
                    else
                    {
                        // ensure the DefaultPage is found within the app
                        spa.Options.DefaultPageStaticFileOptions = fileOptions;
                    }
                });
            });
        }
    }
}