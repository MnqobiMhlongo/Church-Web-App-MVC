using ChurchSystem.Controllers;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChurchSystem.Startup))]
namespace ChurchSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //tupacCantCMe
            GlobalConfiguration.Configuration
               .UseSqlServerStorage("DefaultConnection"); // Use your connection string

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            // Schedule a recurring job
            //RecurringJob.AddOrUpdate(() => SendEventReminders(), Cron.Minutely);

            app.MapSignalR();
            ConfigureAuth(app);
        }
        public void SendEventReminders()
        {
            // Here you need to call the SendEventReminders method from your controller
            var controller = new HomeController();
            controller.SendEventReminders();
        }
    }
}
