using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Mu
{
    public class DataReceiver
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                if (context.Request.Method == "POST")
                {
                    // Read the incoming data from the request body
                    string data = await new StreamReader(context.Request.Body).ReadToEndAsync();

                    // Process the received data
                    // For demonstration, we'll just print it to the console
                    System.Console.WriteLine("Received data: " + data);

                    // Return a response
                    await context.Response.WriteAsync("Data received successfully!");
                }
            });
        }
    }
}
