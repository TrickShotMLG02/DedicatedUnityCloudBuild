using System.Text;
using System.Net;

using DedicatedUnityCloudBuild.Config;
using DedicatedUnityCloudBuild.Log;
using DedicatedUnityCloudBuild.Variables;

namespace DedicatedUnityCloudBuild.WebServer
{
    internal class HttpServer
    {
        // singleton pattern
        public static HttpServer Instance;
        private bool isRunning = false;

        private HttpListener listener;
        private Task connectionListener;
        private string url;
        private int pageViews = 0;
        private int requestCount = 0;
        private string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public HttpServer()
        {
            // check if there is already instance of HttpServer
            if (Instance != null)
            {
                Logger.Instance.LogError("HttpServer Instance already exists!");
            }
            else
            {
                // else set current object as Instance
                Instance = this;

                if (ProgramVariables.verbose)
                    Logger.Instance.LogInfo("Created new HttpServer Instance");

                // Start HttpServer in seperate thread
                new Thread(new ThreadStart(StartServer)).Start();

                //StartServer();
            }
        }

        public void Dispose()
        {
            StopServer();

            if (ProgramVariables.verbose)
                Logger.Instance.LogInfo("Disposed HttpServer Instance");

            Instance = null;
        }

        public async Task HandleIncomingConnections()
        {
            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (isRunning)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                if (ProgramVariables.verbose)
                {
                    String connectionInfo = "Request #" + requestCount + "\n" + req.Url.ToString() + "\n" + req.HttpMethod + "\n" + req.UserHostName + "\n" + req.UserAgent;
                    Logger.Instance.LogBlock("Connection Info", connectionInfo);
                }

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    isRunning = false;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                // Write the response info
                string disableSubmit = !isRunning ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public void StartServer()
        {
            if (!isRunning)
            {
                // change status to running
                isRunning = true;

                // load url from config
                url = ConfigManager.Instance.cfg.WebServerURL + ":" + ConfigManager.Instance.cfg.WebServerPort + "/";

                // Create a Http server and start listening for incoming connections
                listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();

                Logger.Instance.LogInfoBlock("Successfully started HttpServer", "HttpServer is running and listening for connections on " + url);

                // Handle requests
                connectionListener = HandleIncomingConnections();
                connectionListener.GetAwaiter().GetResult();
            }
        }

        public void StopServer()
        {
            if (isRunning)
            {
                Logger.Instance.LogInfo("Stopping HttpServer...");

                isRunning = false;
                connectionListener.Dispose();

                // Close the listener
                listener.Close();
            }
        }
    }
}