using HTECChampionsLeagueAPI.Contexts;
using HTECChampionsLeagueAPI.Enumerations;
using HTECChampionsLeagueAPI.Models;
using HTECChampionsLeagueAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HTECChampionsLeagueAPI
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
            services.AddDbContext<MatchContext>(opt => opt.UseInMemoryDatabase(databaseName: "Matches"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IMatchRepository, MatchRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<MatchContext>();
                AddTestData(context);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private static void AddTestData(MatchContext context)
        {
            var match1 = new Match
            {
                AwayTeam = "Barcelona",
                GroupName = GroupName.H,
                HomeTeam = "Inter",
                KickoffAt = new DateTime(2017, 10, 10, 20, 45, 00),
                LeagueTitle = "Champions league 2016/17",
                Matchday = Matchday.Third,
                Score = "2:1"
            };

            var match2 = new Match
            {
                AwayTeam = "Totenham",
                GroupName = GroupName.H,
                HomeTeam = "PSV",
                KickoffAt = new DateTime(2017, 10, 10, 20, 00, 00),
                LeagueTitle = "Champions league 2016/17",
                Matchday = Matchday.Third,
                Score = "3:0"
            };

            context.Matches.Add(match1);

            context.Matches.Add(match2);

            context.SaveChanges();
        }
    }
}
