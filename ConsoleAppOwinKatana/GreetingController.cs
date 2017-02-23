using System.Web.Http;

namespace ConsoleAppOwinKatana
{
    public class GreetingController : ApiController
    {
        public Greeting Get()
        {
            return new Greeting { Text = "Hola Paco!" };
        }
    }
}
