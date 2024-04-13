namespace Farsica.Template.UI.Web
{
    using Farsica.Template.Data.Entity.Identity;
    using Farsica.Template.Shared.Service;

    using Asp.Versioning;
    using Asp.Versioning.ApiExplorer;

    using Farsica.Framework.Core;
    using Farsica.Framework.Identity;
    using Farsica.Framework.Startup;

    using Microsoft.AspNetCore.HttpLogging;
    using Microsoft.OpenApi.Models;

    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    public class Startup(IConfiguration configuration)
        : Startup<ApplicationUser, ApplicationRole>(new StartupOption
        {
            Configuration = configuration,
            DefaultNamespace = DefaultNamespace,
            Localization = true,
            Authentication = true,
            Antiforgery = true,
            Https = true,
            ErrorCodePrefix = "LS",
            Identity = true,
        })
    {
        public const string DefaultNamespace = "Farsica.Template";

        private static readonly Lazy<IServiceProvider?> ServicesList = new(Framework.Hosting.Host.CreateHost<Startup>([])?.Services);
        private const string AllowCorsPolicy = "allowCorsPolicy";

        public static Lazy<IServiceProvider?> Services
        {
            get
            {
                if (!Globals.CurrentCulture.TwoLetterISOLanguageName.Equals("fa", StringComparison.OrdinalIgnoreCase))
                {
                    CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = Globals.GetCulture("fa");
                }
                return ServicesList;
            }
        }

        protected override void ConfigureServicesCore(IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            _ = services.AddDistributedMemoryCache();

            _ = services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddMvc().AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            _ = services.ConfigureSwagger(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer",
                              }
                          },
                         Array.Empty<string>()
                    },
                });

                var apiVersionDescriptionProvider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    var info = new OpenApiInfo()
                    {
                        Title = "Lens",
                        Version = description.ApiVersion.ToString()
                    };

                    if (description.IsDeprecated)
                    {
                        info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
                    }

                    options.SwaggerDoc(description.GroupName, info);
                }
            });

            var urls = Configuration.GetSection("CorsUrls").GetChildren().Select(t => t.Value).ToArray();
            _ = services.AddCors(options => options.AddPolicy(name: AllowCorsPolicy,
                    policy => policy
                    .WithOrigins(urls!)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

            _ = services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = ApiDataProtectorTokenProviderOptions.GetTokenLifespan(Configuration);

                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Headers.AccessControlAllowCredentials = "true";
                    context.Response.Headers.AccessControlAllowHeaders = "*";
                    context.Response.Headers.AccessControlAllowMethods = "*";
                    context.Response.Headers.AccessControlAllowOrigin = context.Request.Headers.Origin;
                    context.Response.Headers.Vary = "Origin";
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = (context) =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.Headers.AccessControlAllowCredentials = "true";
                    context.Response.Headers.AccessControlAllowHeaders = "*";
                    context.Response.Headers.AccessControlAllowMethods = "*";
                    context.Response.Headers.AccessControlAllowOrigin = context.Request.Headers.Origin;
                    context.Response.Headers.Vary = "Origin";
                    return Task.CompletedTask;
                };
                options.Events.OnValidatePrincipal = async (context) =>
                {
                    ArgumentNullException.ThrowIfNull(context);
                    await context.HttpContext.RequestServices.GetRequiredService<IIdentityService>().ValidatePrincipalAsync(context);
                };
            });

            _ = services.AddHttpLogging(logging => logging.LoggingFields = HttpLoggingFields.All);

            _ = services.AddHealthChecks();
        }

        protected override void ConfigureCore([NotNull] IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI(options =>
                {
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

                    var apiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                    for (var i = 0; i < apiVersionDescriptionProvider.ApiVersionDescriptions.Count; i++)
                    {
                        var description = apiVersionDescriptionProvider.ApiVersionDescriptions[i];
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });
            }
            _ = app.Use(async (context, next) =>
            {
                context.Response.Headers.Server = "";

                await next(context);
            });
            _ = app.UseCors(AllowCorsPolicy);
            _ = app.UseCookiePolicy(new CookiePolicyOptions { Secure = CookieSecurePolicy.Always });
            _ = app.UseHttpLogging();

            _ = app.UseHealthChecks("/healthz");
        }
    }
}
