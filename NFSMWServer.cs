using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NFSServer.UI
{
    public class NFSMWServer
    {
        private readonly HttpListener listener = new HttpListener();
        private bool isRunning = false;

        public void Start(string urlPrefix = "http://+:80/")
        {
            if (isRunning) return;
            listener.Prefixes.Add(urlPrefix);
            listener.Start();
            isRunning = true;
            Task.Run(() => HandleRequests());
        }

        public void Stop()
        {
            if (isRunning)
            {
                    isRunning = false;
                    listener.Stop();
            }

        }

        private async Task HandleRequests()
        {
            while (isRunning)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    string path = context.Request.Url.AbsolutePath;
                    Console.WriteLine("Received: " + path);

                    string responseText = GetFakeResponse(path);
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);

                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.OutputStream.Close();
                }
                catch (HttpListenerException ex)
                {
                    if (isRunning)
                        Console.WriteLine("HttpListenerException: " + ex.Message);
                    else
                        Console.WriteLine("Server stopped gracefully.");
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("Listener has been disposed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private string GetFakeResponse(string path)
        {
            if (path.Contains("autolog") || path.Contains("nucleus"))
            {
                return "{\"status\":\"ok\"}";
            }

            return "NFSMW12 Custom Server Online!";
        }
    }
}