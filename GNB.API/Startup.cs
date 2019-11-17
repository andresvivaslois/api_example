using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using GNB.Domain.ControllerWorkers;
using GNB.Domain.ControllerWorkers.Interfaces;
using GNB.Domain.Helpers;
using GNB.Domain.Helpers.Interfaces;
using GNB.Infrastructure.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GNB.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Used Sqlite because doesn't need any infrastructure (SqlServer with users, permissions, etc) to be executed
            services.AddEntityFrameworkSqlite()
                .AddDbContext<BankContext>(options =>
                {
                    var connString = Configuration.GetConnectionString("BankDatabase");
                    options.UseSqlite(connString);
                }, ServiceLifetime.Scoped);

            services.AddScoped<IBankCW, BankCW>();
            services.AddScoped<IRequestHelper, RequestHelper>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddLog4Net();

            app.UseMvc();



            //Update DB in runtime on start
            using (var db = new BankContext())
            {
                db.Database.Migrate();
            }
        }
    }
}
