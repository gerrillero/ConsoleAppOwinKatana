using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleAppOwinKatana
{
    using System.IO;
    using System.Web.Http;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class Program
    {
        static void Main(string[] args)
        {
            String uri = "http://localhost:8080";

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started!");
                Console.ReadKey();
                Console.WriteLine("Stopping!");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app.Use( async (environment, next) =>
            {
                foreach (var pair in environment.Environment)
                {
                    Console.WriteLine("{0}:{1}", pair.Key, pair.Value);
                }

                await next();
            });


            ConfigureWebApi(app);

            app.Use(async (environment, next) =>
            {
                Console.WriteLine("Requesting: " + environment.Request.Path);
                await next();
                Console.WriteLine("Response: " + environment.Response.StatusCode);
            });

            app.UseHelloWorld();
            //app.Use<HelloWorldComponent>();

            //app.UseWelcomePage();

            //app.Run(ctx =>
            //{
            //    return ctx.Response.WriteAsync("Hola, cómo estás?");
            //});
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi", 
                "api/{controller}/{id}", 
                new { id = RouteParameter.Optional });

            app.UseWebApi(config);
        }
    }

    public static class AppBuilderExtension
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }

    public class HelloWorldComponent
    {
        AppFunc _next;

        public HelloWorldComponent(AppFunc next)
        {
            _next = next;
        }

        public Task Invoke(IDictionary<string, object> enviroment)
        {
            var response = enviroment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteAsync("Hola, cómo estás?");
            }

        }
    }
}
