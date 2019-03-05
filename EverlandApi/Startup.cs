using EverlandApi.Accounts;
using EverlandApi.Accounts.Filters;
using EverlandApi.Accounts.Models;
using EverlandApi.Accounts.Services;
using EverlandApi.Core;
using EverlandApi.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EverlandApi
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
            ConfigureAccounts(services);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void ConfigureAccounts(IServiceCollection services)
        {
            services.Configure<BCryptOptions>(
                Configuration.GetSection("BCrypt")
            );
            services.AddSingleton<IPasswordHasher<Account>, BCryptPasswordHasher<Account>>();
            services.AddScoped<IAuthenticationService<Account>, AuthenticationService>();
            services.AddScoped<RequiresAccount>();

            services.AddDbContext<AccountContext>(
                opt => opt.UseMySql(Configuration.GetConnectionString("Default"))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
