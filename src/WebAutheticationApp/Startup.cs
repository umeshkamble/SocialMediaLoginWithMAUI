using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebAutheticationApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        IConfiguration Configuration { get; }

        IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddFacebook(fb =>
                {
                    fb.AppId = Configuration["OAuth:Facebook:ClientId"];
                    fb.AppSecret = Configuration["OAuth:Facebook:ClientSecret"];
                    fb.SaveTokens = true;
                })
                .AddGoogle(g =>
                {
                    g.ClientId = Configuration["OAuth:Google:ClientId"];
                    g.ClientSecret = Configuration["OAuth:Google:ClientSecret"];
                    g.SaveTokens = true;
                })
                .AddTwitter(t=>
                {
                    t.ConsumerKey = Configuration["OAuth:Twitter:ClientId"];
                    t.ConsumerSecret = Configuration["OAuth:Twitter:ClientSecret"];
                    t.SaveTokens = true;
                });
            //.AddMicrosoftAccount(ms =>
            //{
            //    ms.ClientId = Configuration["OAuth:Google:ClientId"];
            //    ms.ClientSecret = Configuration["ClientSecret"];
            //    ms.SaveTokens = true;
            //})
            //.AddApple(a =>
            //{
            //    a.ClientId = Configuration["ServiceId"];
            //    a.KeyId = Configuration["KeyId"];
            //    a.TeamId = Configuration["TeamId"];
            //    a.UsePrivateKey(keyId
            //        => WebHostEnvironment.ContentRootFileProvider.GetFileInfo($"AuthKey_{keyId}.p8"));
            //    a.SaveTokens = true;
            //});

            /*
            * For Apple signin
            * If you are running the app on Azure App Service you must add the Configuration setting
            * WEBSITE_LOAD_USER_PROFILE = 1
            * Without this setting you will get a File Not Found exception when AppleAuthenticationHandler tries to generate a certificate using your AuthKey_{keyId}.p8 file.
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
